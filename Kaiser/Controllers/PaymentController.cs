
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Core_Layer.Services.CheckOut;
using Microsoft.AspNetCore.Authorization;

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

            var snapshot = await checkOutServices.CheckOutAsync(userId, addressId);

            var authority = await checkOutServices.StartPaymentAsync(userId,snapshot);
            return Ok(new
            {
              url=  $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
            });
        }

        [HttpGet("Payment/Callback")]
        public async Task<IActionResult> PaymentCallback(
            [FromQuery] Guid paymentId,
            [FromQuery]string authority,
            [FromQuery]string status)
        {
        var result = await checkOutServices.HandleCallbackAsync(paymentId, authority, status);
  

         return Ok(result);
        }


}