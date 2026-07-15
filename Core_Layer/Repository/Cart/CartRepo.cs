using AutoMapper;
using Core_Layer.Dtos.CartDto;
using Core_Layer.Dtos.Product;
using Data_Layer.Context;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core_Layer.Repository.Cart;

public class CartRepo(ILogger<CartRepo> logger,Context context,IMapper mapper) :ICartRepo
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
            {
                logger.LogWarning("a user tried to add cart but userId not found");
                return ActionResult.Failed("کاربری پیدا نشد");
            }



            if (user.Carts != null && user.Carts.Any())
            {
                cart = user.Carts.First();
                if (cart.CartItems != null&&cart.CartItems.Any(e=>e.ProductId == itemDto.ProductId))
                {
                    logger.LogInformation("user:{user} added this item:{item} before",itemDto.UserId,itemDto.ProductId);
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
            logger.LogError(e, "Error while Add item to cart {@itemDto}",itemDto);
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
            logger.LogError(e,"Error while deleting Cart Item {cartItemId}",cardItemId);
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
            logger.LogError(e, "error while increase quantity of cart {cardItemId}", cardItemId);
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
            logger.LogError(e, "error while Decrease quantity of cart {carditemId}", cardItemId);
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
                ImageName =  ci.Product.Images.FirstOrDefault().Name ,
                UserId = userId,
                Product = mapper.Map<ProductDto>(ci.Product)
            }).ToListAsync();
        return cartItems;

    }

    public async Task<ActionResult> CheckQuantitiesAsync(string userId)
    {
        try
        {
            // حذف آیتم‌های بدون موجودی با یک کوئری
                 await context.CartItems
                .Where(ci => ci.Cart.UserId == userId &&
                             ci.Product.StockQuantity < 1)
                .ExecuteDeleteAsync();

            // به‌روزرسانی آیتم‌های با موجودی بیشتر از حد
            var itemsToUpdate = await context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userId &&
                             ci.Quantity > ci.Product.StockQuantity &&
                             ci.Product.StockQuantity >= 1)
                .ToListAsync();

            if (itemsToUpdate.Any())
            {
                foreach (var item in itemsToUpdate)
                {
                    item.Quantity = item.Product.StockQuantity;
                }
                await context.SaveChangesAsync();
            }

            return ActionResult.Completed();
        }
        catch (Exception e)
        {
            logger.LogError(e,"Error whi,e check if product quantity with cart quantity for user {userId}",userId);
            return ActionResult.Failed(e.Message);
        }
    }

    public async Task<ActionResult> ClearUserCartAsync(string userId)
    {
        try
        {
            var result = await context.Carts
                .Include(e => e.CartItems)
                .Where(e=>e.UserId == userId)
                .FirstOrDefaultAsync(e => e.UserId == userId);
            if (result == null || result.CartItems == null || !result.CartItems.Any() )
            {
                return ActionResult.Failed("کارتی وجود نداشت");
            }
            context.RemoveRange(result.CartItems);
            await context.SaveChangesAsync();
            return ActionResult.Completed();
           
        }
        catch (Exception e)
        {
            logger.LogError(e,"Error while clear carts for user{userId}",userId);
            return ActionResult.Failed(e.Message);
        }
    }
}