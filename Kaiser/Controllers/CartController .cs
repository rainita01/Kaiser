using Core_Layer.Dtos.CartDto;
using Core_Layer.Repository.Cart;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kaiser.Controllers;

public class CartController(ICartRepo cartRepo) : ControllerBase
{
    [HttpGet("Carts")] 
    public async Task<IActionResult> GetMyCarts()
    {
        var result = await cartRepo.GetUserCartItemsAsync(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        return Ok(result);

    }

    [HttpPost("Carts/AddCart")]
    public async Task<IActionResult> AddCart([FromBody] RequestCartDto dto)
    {
        var cartitem = new AddCartItemDto()
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
        };
        if (string.IsNullOrEmpty(cartitem.UserId))
            return BadRequest("شما باید اول وارد شوید");

        var result = await cartRepo.AddItemAsync(cartitem);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpPut("Carts/IncreaseQuantity")]
    public async Task<IActionResult> IncreaseQuantity(int carItemId)
    {
        var result = await cartRepo.IncreaseQuantityAsync(carItemId);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }

    [HttpPut("Carts/DecreaseQuantity")]
    public async Task<IActionResult> DecreaseQuantity(int carItemId)
    {
        var result = await cartRepo.DecreaseQuantityAsync(carItemId);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }
    [HttpDelete("Carts/RemoveCartItem")]
    public async Task<IActionResult> RemoveCartItem(int cartItemId)
    {
        var result = await cartRepo.RemoveItemAsync(cartItemId);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }





}