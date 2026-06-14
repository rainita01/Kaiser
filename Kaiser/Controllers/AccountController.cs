
using Core_Layer.Dtos.AccountDto;
using Core_Layer.Dtos.AddressDto;
using Core_Layer.Repository.Address;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kaiser.Controllers;

[ApiController]
public class AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IAddressRepo addressRepo) : ControllerBase
{
    [HttpPost("/Account/Login")]
    public async Task<IActionResult> Login([FromBody]LoginUserDto dto)
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
    public async Task<IActionResult> Register([FromBody]RegisterUserDto dto)
    {
        var result = await userManager.CreateAsync(new User()
        {
            AccessFailedCount = 0,
            FirstName = dto.Firstname,
            LastName = dto.Lastname,
            UserName = dto.Username,
            PhoneNumber = dto.PhoneNumber

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
    [HttpPost("Account/Profile/AddAddress")]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var result = await addressRepo.AddAsync(dto);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);

    }
    [HttpGet("UserManager")]
    public IActionResult UserManager()
    {
        var users = userManager.Users.Select(e => new UserDto()
        {
            Id = e.Id,
            Firstname = e.FirstName,
            Lastname = e.LastName,
            Username = e.UserName,
            PhoneNumber = e.PhoneNumber
        }).ToList();
        return Ok(users);
    }


    [HttpGet("UserManager/Edit")]
    public async Task<IActionResult> Edit([FromQuery] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest("کاربر پیدا نشد");
        var userDto = new UserDto()
        {
            Id = user.Id,
            Firstname = user.FirstName,
            Lastname = user.LastName,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber
        };
        return Ok(userDto);
    }

 
  
    [HttpGet("Account/Profile/MyAddresses")]
    public async Task<IActionResult> MyAddresses([FromQuery] string userId)
    {
        var addresses = await addressRepo.GetUserAddresses(userId);
        return Ok(addresses);
    }
    [HttpPut("UserManager/Edit")]
    public async Task<IActionResult> Edit([FromBody] UserDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.Id);
        if (user == null)
            return BadRequest("کاربر پیدا نشد");
        user.FirstName = dto.Firstname ?? user.FirstName;
        user.LastName = dto.Lastname ?? user.LastName;
        user.UserName = dto.Username ?? user.UserName;
        user.PhoneNumber = dto.PhoneNumber;
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest(result.Errors);
        
    }

    [HttpDelete("UserManager/Remove")]
    public async Task<IActionResult> Remove([FromBody] string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return BadRequest("کاربر پیدا نشد");
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);

    }


    public async Task<IActionResult> RemoveAddress(int id)
    {
        var result = await addressRepo.DeleteAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }
}