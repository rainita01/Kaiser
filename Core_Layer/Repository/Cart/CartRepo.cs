using Core_Layer.Dtos.CartDto;
using Core_Layer.Repository.Image;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.Cart;

public class CartRepo(Context context,IImageRepo imageRepo) :ICartRepo
{
    public async Task<ActionResult> AddItemAsync(AddCartItemDto itemDto)
    {
        var transAction = await context.Database.BeginTransactionAsync();
        try
        {
            if (itemDto.Quantity <= 0)
                return ActionResult.Failed("تعداد باید بیشتر از صفر باشد");
            var product = await context.Products.FindAsync(itemDto.ProductId);
            if (product == null)
                return ActionResult.Failed("محصول یافت نشد");
            if (product.StockQuantity < itemDto.Quantity)
                return ActionResult.Failed("موجودی کافی نیست");


            Data_Layer.Entities.Cart cart;
            var user = await context.Users
                .Include(e => e.Carts)
                .ThenInclude(s=>s.CartItems)
                .FirstOrDefaultAsync(e => e.Id == itemDto.UserId);
            if (user == null)
                return ActionResult.Failed("کاربری پیدا نشد");



            if (user.Carts != null && user.Carts.Any())
            {
                cart = user.Carts.First();
                if (cart.CartItems != null&&cart.CartItems.Any(e=>e.ProductId == itemDto.ProductId))
                {
                    return ActionResult.Failed("کاربر قبلا این ایتم را اضافه کرده است ");
                }
            }
            else
            {
                cart = new Data_Layer.Entities.Cart() { UserId = itemDto.UserId };
                context.Carts.Add(cart);
            }
           
            

            if (cart.CartItems == null)
                cart.CartItems = new List<CartItem>();

            cart.CartItems?.Add(new CartItem() {Cart = cart,ProductId =itemDto.ProductId,Quantity = itemDto.Quantity});
            await context.SaveChangesAsync();
            await transAction.CommitAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
           await transAction.RollbackAsync();
          return  ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> RemoveItemAsync(int cardItemId)
    {
        try
        {
            var item = await context.CartItems.FindAsync(cardItemId);
            if (item == null)
                return ActionResult.Failed("ایتم پیدا نشد");
            context.Remove(item);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> IncreaseQuantityAsync(int cardItemId)
    {
        try
        {
            var item = await context.CartItems.FindAsync(cardItemId);
            if (item == null)
                return ActionResult.Failed("ایتم پیدا نشد");
            var product = await context.Products.FindAsync(item.ProductId);

            if (product?.StockQuantity <= item.Quantity)
                return ActionResult.Failed("تعداد درخواستی بیش از موجودیت میباشد ");
                
            

            item.Quantity += 1;
            await context.SaveChangesAsync();

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> DecreaseQuantityAsync(int cardItemId)
    {
        try
        {
            var item = await context.CartItems.FindAsync(cardItemId);
            if (item == null)
                return ActionResult.Failed("ایتم پیدا نشد");
            item.Quantity -= 1;
            if (item.Quantity < 1)
                 context.CartItems.Remove(item);
            
            await context.SaveChangesAsync();

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<List<CartItemDto>> GetUserCartItemsAsync(string userId)
    {
        var cartItems = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.Cart.UserId == userId)  // ارتباط با Cart
            .Select(ci => new CartItemDto
            {
                DiscountPercent = ci.Product.DiscountPercent,
                Id = ci.Id,
                Name = ci.Product.Name,
                Price = ci.Product.Price,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                ImageName =  ci.Product.Images.FirstOrDefault().Name,
                UserId = userId
            }).ToListAsync();

        return cartItems;

    }
}