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

namespace Busines_Layer.Services.CheckOut;

public class CheckoutService(
    Context context,
    IMapper mapper,
    ICartRepo cartRepo,
    IAddressRepo addressRepo,
    ISnapshotRepo snapshotRepo,
    IZarinPalServices zarinPal,
    IProductRepo productRepo,
    IOptions<PaymentOption > paymentOption,
    ILogger<CheckoutService> logger,
    IPostexServices postexServices
    ) : ICheckOutServices
{

    public async Task<CheckoutDto> CheckOutAsync(string userId, int addressId)
    {
       
           var cartItems = await cartRepo.GetUserCartItemsAsync(userId);
           if (cartItems == null || !cartItems.Any())
           {
               logger.LogWarning("no cart found for user{userId}",userId);
               throw new FileNotFoundException("کارتی پیدا نشد");
           }
           var address = await addressRepo.FindAddressAsync(userId, addressId);

          var checkQua =  await cartRepo.CheckQuantitiesAsync(userId);
          if (!checkQua.Success)
          {
              logger.LogError(checkQua.Message);
              throw new Exception();
          }
           var parcels = new List<GetShippingQuotesQueryParcels>();
           foreach (var item in cartItems)
           {
               parcels.Add(new GetShippingQuotesQueryParcels()
               {
                ParcelProperties   = mapper.Map<ParcelPropertyDto>(item),
                ToCityCode = address.city_code,
                PaymentType = "SENDER"
               });
           }

           var shippingPrice = await postexServices.CheckPrice(parcels) ;
           var snapshot = new SnapShot() 
           { 
                 Id = Guid.NewGuid(),
                 PostCode = address.PostCode,
                 City = address.city_name,
                 Province = address.province_name,
                 UserId = userId,
                 State = SnapShotState.Pending,
                 ShippingCost = shippingPrice,
                 CreatedAt = DateTime.UtcNow,
                 Items = mapper.Map<List<SnapShotItem>>(cartItems),
                 FullAddress = address.FullAddress,
                 PhoneNumber = address.PhoneNumber,
           };
            return new CheckoutDto(){
               ShippingPrice = shippingPrice,
               ProductsPrice = (double)cartItems.Sum(e => e.Price * (1 - e.DiscountPercent / 100m) * cartItems.Count),
               Snapshot = snapshot
            };
    }

    public async Task<string> PurchaseAsync(string userId, int addressId)
    {
        
        var checkout = await CheckOutAsync(userId, addressId);
      
        var request = await zarinPal.RequestAsync(new ZarinPalRequestDto()
        {
            Amount = checkout.TotalPrice,
            Description = $"پرداخت سفارش",
            CallbackUrl = paymentOption.Value.CallbackUrl,
            MerchantId = paymentOption.Value.MerchantId
        });
        if (request.data.code != 100)
        {
            logger.LogError("request to zarinpal get failed {request}", request);
            string? erorrs = request.errors?.ToString();
            throw new RequestFailedException(erorrs ?? "error while requesting to zarinpal server");
        }

        checkout.Snapshot.TotalPrice = checkout.TotalPrice;
        checkout.Snapshot.Authority = request.data.authority;
        await context.SnapShots.AddAsync(checkout.Snapshot);
        await context.SaveChangesAsync();
        logger.LogInformation("request to zarin pal completed ");
        return request.data.authority;
        
    }


    public async Task<PaymentResultDto> HandleCallbackAsync(string authority, string status)
    {
        var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (status != "OK")
            {
                await context.SaveChangesAsync();
                logger.LogWarning("payment failed {status},authorize:{authorize}", status, authority);
                return new PaymentResultDto(false, null, "پرداخت لغو شد");
            }
            var snapshot = await snapshotRepo.GetAsync(authority);

            if (snapshot == null)
            {
                logger.LogWarning("payment failed {status},authorize:{authorize}", status, authority);
                return new PaymentResultDto(false, null, "پرداخت لغو شد");
            }
            var verify = await zarinPal.VerifyAsync(new ZarinPalVerifyDto()
            {
                Authority = authority,
                Amount = snapshot.TotalPrice,
                MerchantId = paymentOption.Value.MerchantId
            });
            if (verify.Data.Code != 101 && verify.Data.Code != 100)
            {
                await context.SaveChangesAsync();
                logger.LogWarning("verify purchase failed {@result}", verify);
                return new PaymentResultDto(false, null, "تایید پرداخت ناموفق بود");
            }
            var payment = new Payment()
            {
                Id = Guid.NewGuid(),
                Amount = snapshot.TotalPrice,
                Authority = snapshot.Authorize,
                CreatedAt = DateTime.Now,
                RefId = verify.Data.RefId.ToString(),
                SnapShotId = (Guid)snapshot.Id!
            };
            foreach (var item in snapshot.Items)
            {
                var affected = await productRepo.ExecuteProductQuantityCostAsync(item.ProductId, item.Count);
                if (!affected.Success)
                {
                    payment.State = PaymentState.Failed;
                    await context.SaveChangesAsync();
                    logger.LogWarning("purchase succeed but there is less stock of product than request {itemName} ,{id} موجودی {count}", item.ProductName, item.Id, item.Count);
                    return new PaymentResultDto(true, payment.RefId,
                        "پرداخت موفق بود ولی موجودی محصول کافی نیست، با پشتیبانی تماس بگیرید");
                }
            }
            var order = mapper.Map<Order>(snapshot);
            order.PaymentId = payment.Id;
            await context.Orders.AddAsync(order);
            payment.State = PaymentState.Succeeded;
            payment.VerifiedAt = DateTime.UtcNow;
            var cart = await context.Carts.Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId ==snapshot.UserId);
            if (cart?.CartItems != null)
                context.CartItems.RemoveRange(cart.CartItems);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            logger.LogInformation("purchase succeed {@model}", order);
            return new PaymentResultDto(true, payment.RefId, "خرید با موفقیت انجام شد");

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "error while purchase callback method");
            throw;
        }
    
    }

}



