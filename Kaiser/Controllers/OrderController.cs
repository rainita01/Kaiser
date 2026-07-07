using System.Security.Claims;
using Core_Layer.Repository.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class OrderController(IOrderRepo orderRepo) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpGet("OrderManager/Orders")]
    public async Task<IActionResult> GetOrders()
    {
        var result = await orderRepo.GetAllOrderListAsync();
        return Ok(result);

    }

    [Authorize]
    [HttpGet("MyOrders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("لطفا اول وارد شوید");
        var result = await orderRepo.GetOrderListAsync(userId);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("OrderManager/RemoveOrder")]
    public async Task<IActionResult> RemoveOrder(int orderId)
    {
        var result =await orderRepo.RemoveAsync(orderId);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }
}