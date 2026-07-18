using AutoMapper;
using Azure;

using Core_Layer.Dtos.PaymentDto;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.Cart;
using Core_Layer.Repository.Payment;
using Core_Layer.Repository.Product;
using Core_Layer.Services.Api;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace Core_Layer.Services.CheckOut;

public class CheckoutService(
    Context context,
    IMapper mapper,
    ICartRepo cartRepo,
    IAddressRepo addressRepo,
    IPaymentRepo paymentRepo,
    IZarinPalServices zarinPal,
    IProductRepo productRepo,
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
           await context.SnapShots.AddAsync(snapshot);
           await context.SaveChangesAsync();
           var paymentId = Guid.NewGuid();
            var request = await zarinPal.RequestAsync(new ZarinPalRequestDto()
           {
               MerchantId = "9026f668-323b-416c-94a6-54fdc65b4d34",
               Amount = snapshot.TotalPrice,
               Description = $"پرداخت سفارش {snapshot.Id}",
               CallbackUrl = $"http://localhost:3000/Payment/Callback?paymentId={paymentId}",
           });
           if (request?.data == null || request.data.code != 100)
           {
               logger.LogError("request to zarinpal get failed {request}",request);
               throw new RequestFailedException(request.errors.ToString());
           }
           var payment = new Payment
           {
               Id = paymentId,
               SnapShotId = snapshot.Id,
               Amount = snapshot.TotalPrice,
               State = PaymentState.Requested,
               CreatedAt = DateTime.UtcNow,
               Authority = request.data.authority
           };
           await context.Payments.AddAsync(payment);
           await context.SaveChangesAsync();
           await transaction.CommitAsync();
           logger.LogInformation("request to zarin pal completed {@payment}",payment);
           return payment.Authority;

       }
       catch (Exception e)
       { logger.LogError(e,"error in method checkout for user : {userId}",userId);
         await  transaction.RollbackAsync();
         throw;
       }
    }

    public async Task<PaymentResultDto> HandleCallbackAsync(Guid paymentId, string authority, string status)
    {

        var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var payment = await paymentRepo.GetPaymentAsync(paymentId, authority);

            if (payment.State == PaymentState.Succeeded)
            {
                logger.LogWarning("payment succeeded before {status},authorize:{authorize},payment: {payment}", status, authority, paymentId);
                return new PaymentResultDto(true, payment.RefId, "قبلا تایید شده");
            }
            if (payment.State == PaymentState.Failed)
            {
                logger.LogWarning("payment failed {status},authorize:{authorize},payment: {payment}", status, authority, paymentId);
                return new PaymentResultDto(false, null, "پرداخت ناموفق بود");
            }

            if (status != "OK")
            {
                payment.State = PaymentState.Failed;
                await context.SaveChangesAsync();
                logger.LogWarning("payment failed {status},authorize:{authorize},payment: {payment}",status,authority,paymentId);
                return new PaymentResultDto(false, null, "پرداخت لغو شد");
            }
            var verify = await zarinPal.VerifyAsync(new ZarinPalVerifyDto()
            {
                MerchantId = "9026f668-323b-416c-94a6-54fdc65b4d34",
                Authority = authority,
                Amount = payment.Amount
            });
            if (verify.Data?.Code != 100 && verify.Data?.Code != 101)
            {
                payment.State = PaymentState.Failed;
                await context.SaveChangesAsync();
                logger.LogWarning("verify purchase failed {@result}",verify);
                return new PaymentResultDto(false, null, "تایید پرداخت ناموفق بود");
            }
            foreach (var item in payment.SnapShot.Items)
            {
                var affected = await productRepo.ExecuteProductQuantityCostAsync(item.ProductId, item.Count);
                if (!affected.Success)
                {

                    payment.State = PaymentState.Succeeded;
                    payment.RefId = verify.Data.RefId.ToString();
                    payment.VerifiedAt = DateTime.UtcNow;
                    await context.SaveChangesAsync();
                    await transaction.RollbackAsync();
                    logger.LogWarning("purchase succeed but there is less stock of product than request {itemName} ,{id} موجودی {count}", item.ProductName, item.Id,item.Count);
                    return new PaymentResultDto(true, payment.RefId,
                        "پرداخت موفق بود ولی موجودی محصول کافی نیست، با پشتیبانی تماس بگیرید");
                }
            }
            var order = mapper.Map<Order>(payment.SnapShot);
            order.PaymentId = payment.Id;
            await context.Orders.AddAsync(order);
            payment.State = PaymentState.Succeeded;
            payment.RefId = verify.Data.RefId.ToString();
            payment.VerifiedAt = DateTime.UtcNow;
            payment.SnapShot.State = SnapShotState.Converted;
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


