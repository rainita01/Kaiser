
using AutoMapper;
using Azure;
using Busines_Layer.Dtos.PaymentDto;
using Busines_Layer.Dtos.Postex;
using Busines_Layer.Repository.Address;
using Busines_Layer.Repository.Cart;
using Busines_Layer.Repository.Product;
using Busines_Layer.Repository.Sanpshot;
using Busines_Layer.Services.Api;
using Busines_Layer.Services.Api.Postex;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Busines_Layer.Dtos.AddressDto;
using Busines_Layer.Dtos.CartDto;
using Busines_Layer.Dtos.SnapShotDto;

namespace Busines_Layer.Services.CheckOut;

public class CheckoutService(
    Context context,
    IMapper mapper,
    ICartRepo cartRepo,
    IAddressRepo addressRepo,
    ISnapshotRepo snapshotRepo,
    IZarinPalServices zarinPal,
    IProductRepo productRepo,
    IOptions<PaymentOption> paymentOption,
    ILogger<CheckoutService> logger,
    IPostexServices postexServices
) : ICheckOutServices
{
    public async Task<CheckoutDto> CheckOutAsync(string userId, int addressId)
    {
        var cartItems = await cartRepo.GetUserCartItemsAsync(userId);
        var address = await GetAddressAsync(userId, addressId);
        await ValidateCartAsync(userId);
        var shippingPrice = await CalculateShippingAsync(cartItems, address);
        var productsPrice = (double)CalculateProductsPrice(cartItems);
        var totalPrice = productsPrice + shippingPrice;
        var snapshot = CreateSnapshot(userId, address, cartItems, shippingPrice, totalPrice);

        return new CheckoutDto
        {
            ProductsPrice = (double)productsPrice,
            ShippingPrice = shippingPrice,
            Snapshot = snapshot
        };
    }

    public async Task<string> PurchaseAsync(string userId, int addressId)
    {
        var checkout = await CheckOutAsync(userId, addressId);

        var paymentRequest = await RequestPaymentAsync(checkout.TotalPrice);

        if (paymentRequest.data.code != 100)
        {
            logger.LogError("ZarinPal payment request failed.");
            var errors = paymentRequest.errors?.ToString();
            throw new RequestFailedException(errors ?? "خطا در ایجاد درخواست پرداخت");
        }

        checkout.Snapshot.TotalPrice = checkout.TotalPrice;

        checkout.Snapshot.Authority = paymentRequest.data.authority;

        await context.SnapShots.AddAsync(checkout.Snapshot);

        await context.SaveChangesAsync();

        logger.LogInformation("Payment request created successfully. ");

        return paymentRequest.data.authority;
    }


    // =========================================================
    // PAYMENT CALLBACK
    // =========================================================

    public async Task<PaymentResultDto> HandleCallbackAsync(string authority, string status)
    {
        // Payment cancelled
        if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Payment cancelled. Authority: {Authority}", authority);

            return new PaymentResultDto(false, null, "پرداخت لغو شد");
        }

        var snapshot = await snapshotRepo.GetAsync(authority);

        if (snapshot == null)
        {
            logger.LogWarning("Snapshot not found. Authority: {Authority}", authority);

            return new PaymentResultDto(false,null, "اطلاعات سفارش پیدا نشد");
        }

        var alreadyProcessed = await context.Payments.AnyAsync(x => x.Authority == authority);
        if (alreadyProcessed)
        {
            logger.LogWarning("Payment already processed. Authority: {Authority}", authority);

            return new PaymentResultDto(true, authority, "این پرداخت قبلاً پردازش شده است");
        }
        var verifyResult = await VerifyPaymentAsync(authority, snapshot.TotalPrice);

        if (verifyResult.Data.Code != 100 && verifyResult.Data.Code != 101)
        {
            logger.LogWarning("Payment verification failed.");

            return new PaymentResultDto(false, null, "تایید پرداخت ناموفق بود");
        }
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var payment = CreatePayment(snapshot, verifyResult.Data.RefId);
            var stockResult = await DecreaseStockAsync(snapshot);

            if (!stockResult.Success)
            {
                payment.State = PaymentState.Failed;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.LogWarning("Payment succeeded but stock was insufficient. Message: {Message}", stockResult.Message);
                return new PaymentResultDto(true, payment.RefId, "پرداخت موفق بود ولی موجودی محصول کافی نیست، با پشتیبانی تماس بگیرید");
            }
            var order = mapper.Map<Order>(snapshot);

            order.PaymentId = payment.Id;
            await context.Orders.AddAsync(order);
            payment.State = PaymentState.Succeeded;
            payment.VerifiedAt = DateTime.UtcNow;


      
            await ClearCartAsync(snapshot.UserId);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInformation("Purchase completed successfully.");

            return new PaymentResultDto(true, payment.RefId, "خرید با موفقیت انجام شد");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error while processing payment callback. Authority: {Authority}", authority);
            throw;
        }
    }


    private async Task ValidateCartAsync(string userId)
    {
        var result = await cartRepo.CheckQuantitiesAsync(userId);

        if (!result.Success)
        {
            logger.LogWarning("Cart quantity validation failed. UserId: {UserId}, Message: {Message}", userId, result.Message);
            throw new InvalidOperationException(result.Message);
        }
    }
    private async Task<AddressDto> GetAddressAsync(string userId, int addressId)
    {
       return await addressRepo.FindAddressAsync(userId, addressId);
       
    }


    // =========================================================
    // SHIPPING
    // =========================================================

    private async Task<double> CalculateShippingAsync(IEnumerable<CartItemDto> cartItems, AddressDto address)
    {
        var parcels = BuildParcels(cartItems, address);
        return await postexServices.CheckPrice(parcels);
    }


    private List<GetShippingQuotesQueryParcels> BuildParcels(IEnumerable<CartItemDto> cartItems, AddressDto address)
    {
        var parcels = new List<GetShippingQuotesQueryParcels>();

        foreach (var item in cartItems)
        {
            for (var i = 0;
                 i < item.Quantity;
                 i++)
            {
                parcels.Add(new GetShippingQuotesQueryParcels
                    {
                        ParcelProperties = mapper.Map<ParcelPropertyDto>(item),
                        ToCityCode = address.city_code,
                        PaymentType =
                            "SENDER"
                    });
            }
        }

        return parcels;
    }


    // =========================================================
    // PRICE
    // =========================================================

    private decimal CalculateProductsPrice(IEnumerable<CartItemDto> cartItems)
    {
        return cartItems.Sum(item => item.Price * (1 - item.DiscountPercent / 100m) * item.Quantity);
    }


    // =========================================================
    // SNAPSHOT
    // =========================================================

    private SnapShot CreateSnapshot(string userId, AddressDto address, IEnumerable<CartItemDto> cartItems, double shippingPrice, double totalPrice)
    {
        return new SnapShot
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PostCode = address.PostCode,
            City = address.city_name,
            Province = address.province_name,
            FullAddress = address.FullAddress,
            PhoneNumber = address.PhoneNumber,
            ShippingCost = shippingPrice,
            TotalPrice = totalPrice,
            State = SnapShotState.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = mapper.Map<List<SnapShotItem>>(cartItems)
        };
    }


    // =========================================================
    // ZARINPAL
    // =========================================================

    private async Task<ZarinPalResponseDto> RequestPaymentAsync(double amount)
    {
        return await zarinPal.RequestAsync(new ZarinPalRequestDto
            {
                Amount = amount,
                Description = "پرداخت سفارش",
                CallbackUrl = paymentOption.Value.CallbackUrl,

                MerchantId = paymentOption.Value.MerchantId
            });
    }


    private async Task<ZarinPalVerifyResponseDto> VerifyPaymentAsync(string authority, long amount)
    {
        return await zarinPal.VerifyAsync(
            new ZarinPalVerifyDto
            {
                Authority = authority,
                Amount = amount,
                MerchantId = paymentOption.Value.MerchantId
            });
    }


    // =========================================================
    // PAYMENT
    // =========================================================

    private Payment CreatePayment(SnapShotDto snapshot, long refId)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            Amount = snapshot.TotalPrice,
            Authority = snapshot.Authority,
            CreatedAt = DateTime.UtcNow,
            RefId = refId.ToString(),
            SnapShotId = (Guid)snapshot.Id,
            State = PaymentState.Requested
        };
    }


    // =========================================================
    // INVENTORY
    // =========================================================

    private async Task<(bool Success, string? Message)> DecreaseStockAsync(SnapShotDto snapshot)
    {
        foreach (var item in snapshot.Items)
        {
            var result = await productRepo.ExecuteProductQuantityCostAsync(item.ProductId, item.Count);

            if (!result.Success)
            {
                logger.LogWarning("Insufficient stock. ProductId: {ProductId}, ProductName: {ProductName}, Requested: {Count}", item.ProductId, item.ProductName, item.Count);
                return (false, $"موجودی محصول {item.ProductName} کافی نیست");
            }
        }

        return (true, null);
    }

    private async Task ClearCartAsync(string userId) 
    {
        var cart =
            await context.Carts
                .Include(x => x.CartItems)
                .FirstOrDefaultAsync(x => x.UserId == userId);

        if (cart?.CartItems == null || !cart.CartItems.Any())
        {
            return;
        }
        context.CartItems.RemoveRange(cart.CartItems);
    }
}