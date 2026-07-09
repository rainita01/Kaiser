using AutoMapper;
using Azure;
using Core_Layer.Dtos.PaymentDto;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.Cart;
using Core_Layer.Services.Api;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;



namespace Core_Layer.Services.CheckOut;

public class CheckoutService(
    Context context,
    IMapper mapper,
    ICartRepo cartRepo,
    IAddressRepo addressRepo,
    IZarinPalServices zarinPal) : ICheckOutServices
{

    public async Task<Guid> CheckOutAsync(string userId, int addressId)
    {
        var cartItems = await cartRepo.GetUserCartItemsAsync(userId);
            if ( cartItems == null || !cartItems.Any())
                throw new FileNotFoundException("کارتی پیدا نشد");
            var address = await context.Addresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId);
            if (address == null)
                throw new SqlNullValueException("ادرس پیدا نشد");
            foreach (var item in cartItems)
            {
                if (item.Quantity> item.Product.StockQuantity)    
                {
                    throw new InvalidOperationException(
                        $"مقدار موجودی کمتر از درخواستیه شماست. موجودی:{item.Product.StockQuantity}");
                }   
            }
            var snapshot = new SnapShot() 
            { 
                Id = Guid.NewGuid(),
                AddressId = addressId,
                UserId = userId,
                State = SnapShotState.Pending,
                ShippingCost = 1000000,
                CreatedAt = DateTime.UtcNow,
                Items = mapper.Map<List<SnapShotItem>>(cartItems),
                
            };

            snapshot.TotalPrice = (long)snapshot.Items.Sum(e =>e.UnitPrice * (1 - e.Discount / 100m) * e.Count) + snapshot.ShippingCost;
        
            await context.SnapShots.AddAsync(snapshot);
            await context.SaveChangesAsync();
            
            return snapshot.Id;
    }

    public async Task<string> StartPaymentAsync(string userId,Guid snapshotId)
    {

        var snapshot = await context.SnapShots
            .FirstOrDefaultAsync(s => s.Id == snapshotId && s.UserId == userId);

        if (snapshot == null)
            throw new Exception("سفارش یافت نشد");

        if (snapshot.State == SnapShotState.Converted)
            throw new InvalidOperationException("این سفارش قبلاً پرداخت شده است");
        if (snapshot.State == SnapShotState.Expired)
            throw new InvalidOperationException("مهلت این سفارش تمام شده، لطفاً دوباره از سبد خرید اقدام کنید");

        if (DateTime.UtcNow - snapshot.CreatedAt > TimeSpan.FromMinutes(30))
        {
            snapshot.State = SnapShotState.Expired;
            await context.SaveChangesAsync();
            throw new InvalidOperationException("مهلت این سفارش تمام شده، لطفاً دوباره از سبد خرید اقدام کنید");
        }
        var pendingPayment = await context.Payments
            .Where(p => p.SnapShotId == snapshot.Id && p.State == PaymentState.Requested)
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        if (pendingPayment?.Authority != null)
            return pendingPayment.Authority;

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            SnapShotId = snapshot.Id,
            Amount = snapshot.TotalPrice ,
            State = PaymentState.Requested,
            CreatedAt = DateTime.UtcNow
        };

        var request = await zarinPal.RequestAsync(new ZarinPalRequestDto()
            {
                MerchantId = "9026f668-323b-416c-94a6-54fdc65b4d34",
                Amount = snapshot.TotalPrice,
                Description = $"پرداخت سفارش {snapshot.Id}",
                CallbackUrl = $"https://localhost:7259/Payment/Callback?paymentId={payment.Id}",
            });

            if (request?.data == null || request.data.code != 100)
                throw new RequestFailedException(request.errors.ToString());

        payment.Authority = request.data.authority;
        await context.Payments.AddAsync(payment);
        await context.SaveChangesAsync();

        return payment.Authority;
    }

    public async Task<PaymentResultDto> HandleCallbackAsync(Guid paymentId, string authority, string status)
    {

        var payment = await context.Payments
            .Include(p => p.SnapShot)
            .ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.Authority == authority);

        if (payment == null)
            throw new Exception("پرداخت یافت نشد");


        if (payment.State == PaymentState.Succeeded)
            return new PaymentResultDto(true, payment.RefId, "قبلا تایید شده");
        if (payment.State == PaymentState.Failed)
            return new PaymentResultDto(false, null, "پرداخت ناموفق بود");

        if (status != "OK")
        {
            payment.State = PaymentState.Failed;
            await context.SaveChangesAsync();
            // توجه: SnapShot همچنان Pending می‌مونه، کاربر می‌تونه دوباره از StartPaymentAsync تلاش کنه
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
            return new PaymentResultDto(false, null, "تایید پرداخت ناموفق بود");
        }
        var snapshot = payment.SnapShot;
        foreach (var item in snapshot.Items)
        {
            var affected = await context.Products
                .Where(p => p.Id == item.ProductId && p.StockQuantity >= item.Count)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.StockQuantity, p => p.StockQuantity - item.Count));

            if (affected == 0)
            {

                payment.State = PaymentState.Succeeded;
                payment.RefId = verify.Data.RefId.ToString();
                payment.VerifiedAt = DateTime.UtcNow;
                await context.SaveChangesAsync();
                return new PaymentResultDto(true, payment.RefId,
                    "پرداخت موفق بود ولی موجودی محصول کافی نیست، با پشتیبانی تماس بگیرید");
            }
        }
        var order = mapper.Map<Order>(snapshot);
        order.PaymentId = payment.Id;
        await context.Orders.AddAsync(order);
        payment.State = PaymentState.Succeeded;
        payment.RefId = verify.Data.RefId.ToString();
        payment.VerifiedAt = DateTime.UtcNow;

        snapshot.State = SnapShotState.Converted;

        var cart = await context.Carts.Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == snapshot.UserId);
        if (cart?.CartItems != null)
            context.CartItems.RemoveRange(cart.CartItems);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return new PaymentResultDto(true, payment.RefId, "قبلا تایید شده");
        }

        return new PaymentResultDto(true, payment.RefId, "پرداخت موفق");

    }
}


