
using Busines_Layer.Services.CheckOut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kaiser.Controllers;

[Authorize]
public class PaymentController(ICheckOutServices checkOutServices) : ControllerBase
{
        [HttpPost("Payment/CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var checkout =await checkOutServices.CheckOutAsync(userId, addressId);

            return Ok(new
            { 
                checkout.ShippingPrice,
                checkout.ProductsPrice,
                checkout.TotalPrice
            });
        }

        [HttpGet("Payment/Callback")]
        public async Task<IActionResult> PaymentCallback(
            [FromQuery]string authority,
            [FromQuery]string status)
        {
        var result = await checkOutServices.HandleCallbackAsync(authority, status);
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result.Message);
        }
        [HttpPost("Purchase")]
        public async Task<IActionResult> Purchase([FromBody] int addressId)
        {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();
        var authority = await checkOutServices.PurchaseAsync(userId, addressId);

        return Ok(new
        {
            url = $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
        });

    }


}