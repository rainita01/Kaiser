
using Busines_Layer.Dtos.Postex;
using Busines_Layer.Services.CheckOut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Busines_Layer.Services.Api.Postex;

namespace Kaiser.Controllers;

[Authorize]
public class PaymentController(ICheckOutServices checkOutServices,IPostexServices postexServices) : ControllerBase
{
        [HttpPost("Payment/CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody] int addressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var authority = await checkOutServices.CheckOutAsync(userId, addressId);

            return Ok(new
            {
              url=  $"https://sandbox.zarinpal.com/pg/StartPay/{authority}"
            });
        }

        [HttpGet("Payment/Callback")]
        public async Task<IActionResult> PaymentCallback(
            [FromQuery]string authority,
            [FromQuery]string status)
        {
        var result = await checkOutServices.HandleCallbackAsync(authority, status);
         return Ok(result);
        }
        [HttpPost("ShippingCost")]
        public async Task<IActionResult> CheckShippingPrice()
        {
            var parcels = new List<GetShippingQuotesQueryParcels>();
            parcels.Add(new GetShippingQuotesQueryParcels
            {
                ToCityCode = 1,
                ParcelProperties = new ParcelPropertyDto()
                {
                    BoxTypeId = 7,
                    Height = 20,
                    Length = 20,
                    Width = 20,
                    TotalWeight = 1000,
                    TotalValue = 10_000_000
                },
                PaymentType = "SENDER"
            });
            var shipping = new GetShippingQuotesQueryDto()
            {
                FromCityCode = 266,
                Parcels = parcels,
                CollectionType = "courier_drop_off",
                ValueAddedServices = new OptionalServices()
                {
                    RequestLabel = false,
                    RequestPackaging = false,
                    RequestSmsNotification = false,
                },
                Courier = new Courier()
                {
                    ServiceType = "EXPRESS",
                    CourierCode = "IR_POST"
                }
            };
            var result = postexServices.CheckPrice(shipping);
            return Ok(result);

        }


}