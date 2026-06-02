using Core_Layer.Dtos.AccountDto;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

public class AccountController(UserManager<User> userManager,SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("/Account/Login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await signInManager.PasswordSignInAsync(dto.Username, dto.Password, dto.IsPersistent, false);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.ToString());
        }
    }
    [HttpPost("Account/Register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await userManager.CreateAsync(new User()
        {
            AccessFailedCount = 0,
            FirstName = dto.Firstname,
            LastName = dto.Lastname,
            UserName = dto.Username,

        }, dto.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest(result.ToString());
        }
    }

    [HttpPost("Account/LogOut")]
    public async Task<IActionResult> LogOut()
    {
       await signInManager.SignOutAsync();
        return base.SignOut();  
    }
}