using System.Security.Claims;
using Core_Layer.Repository.Order;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class OrderController(IOrderRepo orderRepo) : ControllerBase
{
    [HttpGet("CheckOut")]
    public async Task<IActionResult> CheckOut(int addressId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return BadRequest("باید اول وارد بشید");
        var result = await orderRepo.CheckOutAsync(userId, addressId);

        return Ok(result);

    }


}