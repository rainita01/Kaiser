using AutoMapper;
using Azure;
using Core_Layer.Dtos.PaymentDto;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.Cart;
using Core_Layer.Repository.Product;
using Core_Layer.Repository.Sanpshot;
using Core_Layer.Services.Api;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Core_Layer.Services.CheckOut;

public class CheckoutService(
    Context context,
    IMapper mapper,
    ICartRepo cartRepo,
    IAddressRepo addressRepo,
    ISnapshotRepo snapshotRepo,
    IZarinPalServices zarinPal,
    IProductRepo productRepo,
    IOptions<PaymentOption > paymentOption,
    ILogger<CheckoutService> logger) : ICheckOutServices
{

    public async Task<string> CheckOutAsync(string userId, int addressId)
    {
       var transaction = await context.Database.BeginTransactionAsync();
       try
       {
           var cartItems = await cartRepo.GetUserCartItemsAsync(userId);
           if (cartItems == null || !cartItems.Any())
           {
               logger.LogWarning("no cart found for user{userId}",userId);
               throw new FileNotFoundException("کارتی پیدا نشد");
           }
           var address = await addressRepo.FindAddressAsync(userId, addressId);

           await cartRepo.CheckQuantitiesAsync(userId);
           var snapshot = new SnapShot()
           {
               Id = Guid.NewGuid(),
               PostCode = address.PostCode,
               City = address.City,
               Province = address.Province,
               UserId = userId,
               State = SnapShotState.Pending,
               ShippingCost = 1000000,
               CreatedAt = DateTime.UtcNow,
               Items = mapper.Map<List<SnapShotItem>>(cartItems),
               FullAddress = address.FullAddress,
               PhoneNumber = address.PhoneNumber,
           };
           snapshot.TotalPrice = (long)snapshot.Items.Sum(e => e.UnitPrice * (1 - e.Discount / 100m) * e.Count) + snapshot.ShippingCost;
            var request = await zarinPal.RequestAsync(new ZarinPalRequestDto()
           {
               Amount = snapshot.TotalPrice,
               Description = $"پرداخت سفارش {snapshot.Id}",
               CallbackUrl = paymentOption.Value.CallbackUrl,
               MerchantId = paymentOption.Value.MerchantId
           });
            if (request.data.code != 100)
           {
               logger.LogError("request to zarinpal get failed {request}",request);
               string? erorrs = request.errors?.ToString();
               throw new RequestFailedException(erorrs?? "error while requesting to zarinpal server");
           }
           snapshot.Authority = request.data.authority;
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           logger.LogInformation("request to zarin pal completed ");
           return request.data.authority;
       }
       catch (Exception e)
       { logger.LogError(e,"error in method checkout for user : {userId}",userId);
         await  transaction.RollbackAsync();
         throw;
       }
    }

    public async Task<PaymentResultDto> HandleCallbackAsync(string authority, string status)
    {

        var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            if (status != "OK")
            {
                await context.SaveChangesAsync();
                logger.LogWarning("payment failed {status},authorize:{authorize}",status,authority);
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
            if (verify.Data.Code != 101)
            {
                await context.SaveChangesAsync();
                logger.LogWarning("verify purchase failed {@result}",verify);
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
                    logger.LogWarning("purchase succeed but there is less stock of product than request {itemName} ,{id} موجودی {count}", item.ProductName, item.Id,item.Count);
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
                .FirstOrDefaultAsync(c => c.UserId == payment.SnapShot.UserId);
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
            logger.LogError(e,"error while purchase callback method");
            throw;
        }
    }
}


