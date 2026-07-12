using Core_Layer.Dtos.AddressDto;
using Core_Layer.Repository.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kaiser.Controllers;

public class AddressController(IAddressRepo addressRepo) : ControllerBase
{
    [Authorize(Roles = "admin")]
    [HttpPost("AddressManager/AddCity")]
    public async Task<IActionResult> AddCity(string cityName,int provinceId)
    {
      var result =  await addressRepo.AddCityAsync(cityName,provinceId);
      if (result.Success)
      {
          return Ok();
      }
      return BadRequest(result.Message);
    }
    [Authorize(Roles = "admin")]
    [HttpPost("AddressManager/AddProvince")]
    public async Task<IActionResult> AddProvince(string provinceName)
    {
        var result = await addressRepo.AddProvinceAsync(provinceName);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }
    [Authorize(Roles = "admin")]
    [HttpDelete("AddressManager/DeleteProvince")]
    public async Task<IActionResult> DeleteProvince(int id)
    {
        var result = await addressRepo.DeleteProvinceAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }
    [Authorize(Roles = "admin")]
    [HttpDelete("AddressManager/DeleteCity")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var result = await addressRepo.DeleteCityAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }

    [HttpGet("Address/GetProvince")]
    public async Task<IActionResult> GetProvinces()
    {
        var result = await addressRepo.GetProvinceAsync();
        return Ok(result);
            
    }
   
    [HttpGet("Address/GetCities")]
    public async Task<IActionResult> GetCities()
    {
        var result = await addressRepo.GetCitiesAsync();
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
            return Ok();
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