using Core_Layer.Dtos.ContactUsDto;
using Core_Layer.Repository.ContactUs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class ContactUsController(IContactUsRepo contactUsRepo) : ControllerBase
{
    [HttpGet("Suggestions")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetSuggestions()
    {
        return Ok(await contactUsRepo.Get());
    }

    [HttpPost("SendSuggestion")]
    public async Task<IActionResult> Send([FromBody]ContactUsDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("لطفا مدل را کامل ارسال کنید");
        }

        var result = await contactUsRepo.Add(dto);

        if (result.Success)
        {
            return Ok();
        }

        return BadRequest("ناموفق در ارسال");
    }
    [HttpDelete("RemoveContactUs")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Remove(int id)
    {
        var result = await contactUsRepo.Remove(id);
        if (result.Success)
        {
            return Ok();
        }
        
        return BadRequest("ناموفق در حذف");
    }

}