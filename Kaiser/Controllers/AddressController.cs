using Busines_Layer.Dtos.AddressDto;
using Busines_Layer.Repository.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Busines_Layer.Services.Api.Postex;

namespace Kaiser.Controllers;

public class AddressController(IAddressRepo addressRepo,IPostexServices postexServices) : ControllerBase
{

    [HttpGet("Address/GetProvince")]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await postexServices.GetProvincesAsync();
        return Ok(result);
            
    }
    
    [HttpGet("Address/GetProvinceCities")]
    public async Task<IActionResult> GetProvinceCities(int provinceCode)
    {
        var result = await postexServices.GetCityByProvince(provinceCode);
        return Ok(result);

    }

    [Authorize]
    [HttpGet("Address/MyAddresses")]
    public async Task<IActionResult> MyAddresses()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return BadRequest("اول باید ورود کنید");
        var addresses = await addressRepo.GetUserAddresses(userId);
        return Ok(addresses);
    }
    [Authorize]
    [HttpPost("Address/AddAddress")]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var userid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userid == null)
            return BadRequest("اول باید وارد شوید");
        var result = await addressRepo.AddAsync(dto, userid);
        if (result.Success)
        {
            return Created();
        }
        return BadRequest(result.Message);
    }

    [Authorize]
    [HttpPut("Address/EditAddress")]
    public async Task<IActionResult> Edit([FromBody] UpdateAddressDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest("مدل ناقص میباشد");
        var result = await addressRepo.UpdateAsync(dto);
        if (result.Success)
        {
            return Ok();
        }

        return BadRequest(result.Message);
    }
    [Authorize]
    [HttpDelete("Address/RemoveAddress")]
    public async Task<IActionResult> RemoveAddress([FromQuery] int id)
    {
        var result = await addressRepo.DeleteAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }
}