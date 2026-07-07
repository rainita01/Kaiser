
using System.Security.Claims;
using Core_Layer.Dtos.AccountDto;
using Core_Layer.Dtos.AddressDto;
using Core_Layer.Repository.Address;
using Core_Layer.Repository.User;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Kaiser.Controllers;

[ApiController]
public class AccountController(UserManager<User> userManager,
    SignInManager<User> signInManager,
    IAddressRepo addressRepo,
    RoleManager<Role> roleManager,
    IUserRepo userRepo
    ) : ControllerBase
{
    [HttpPost("/Account/Login")]
    public async Task<IActionResult> Login([FromBody]LoginUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest("مدل ناقص میباشد");
        var result = await signInManager.PasswordSignInAsync(dto.Username, dto.Password, dto.IsPersistent, false);
        if (result.Succeeded)
        {
            return Ok();
        }
       
        return BadRequest(result.ToString());
        
    }
    [HttpPost("Account/Register")]
    public async Task<IActionResult> Register([FromBody]RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest("مدل ارسالی ناقص میباشد");
        
        var result = await userManager.CreateAsync(new User()
        {
            AccessFailedCount = 0,
            FirstName = dto.Firstname,
            LastName = dto.Lastname,
            UserName = dto.Username,
            PhoneNumber = dto.PhoneNumber,

        }, dto.Password);
        if (result.Succeeded)
        {
            return Ok();
        }
        
        return BadRequest(result.ToString());
    }
    [Authorize(Roles = "admin")]
    [HttpGet("Account/AdminCheck")]
    public IActionResult Check()
    {
        return Ok();
    }

    [Authorize]
    [HttpPost("Account/LogOut")]
    public async Task<IActionResult> LogOut()
    {
      await signInManager.SignOutAsync();
      return Ok("با موفقیت خروج یافت");
    }
   

    [Authorize(Roles = "admin")]
    [HttpGet("UserManager")]
    public async Task<IActionResult> UserManager()
    {
        var result = await userRepo.GetUsersWithRoles();
        return Ok(result);
    }

    [Authorize(Roles = "admin")]
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

    [Authorize(Roles = "admin")]
    [HttpPut("UserManager/Edit")]
    public async Task<IActionResult> Edit([FromBody] UserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("مدل ناقص میباشد");
        }
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
    [Authorize(Roles = "admin")]
    [HttpPut("UserManager/AddRoleToUser")]
    public async Task<IActionResult> AddRoleToUser([FromBody] RoleToUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("مدل ناقص میباشد");
        }

        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return BadRequest("اضافه کردن نقش موفقیت آمیز نبود");


        var result = await userManager.AddToRoleAsync(user, dto.RoleName);
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors.ToString());
    }

    [HttpPut("UserManager/RemoveRoleFromUser")]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RoleToUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("مدل ناقص میباشد");
        }

        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return BadRequest("حذف کردن نقش موفقیت آمیز نبود");
        var result = await userManager.RemoveFromRoleAsync(user, dto.RoleName);
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors.ToString());

    }
    [Authorize(Roles = "admin")]
    [HttpDelete("UserManager/RemoveUser")]
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

    [Authorize]
    [HttpGet("Account/Profile/MyAddresses")]
    public async Task<IActionResult> MyAddresses()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return BadRequest("اول باید ورود کنید");
        var addresses = await addressRepo.GetUserAddresses(userId);
        return Ok(addresses);
    }
    [Authorize]
    [HttpPost("Account/Profile/AddAddress")]
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
    [HttpPut("Account/Profile/EditAddress")]
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
    [HttpDelete("Account/Profile/RemoveAddress")]
    public async Task<IActionResult> RemoveAddress([FromQuery] int id)
    {
        var result = await addressRepo.DeleteAsync(id);
        if (result.Success)
        {
            return Ok();
        }
        return BadRequest(result.Message);
    }
    [Authorize(Roles = "admin")]
    [HttpGet("RoleManager")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleManager.Roles.Select(e
            =>new RoleDto(){Id = e.Id,Name = e.Name}).ToListAsync();
        return Ok(roles);
    }
    [Authorize(Roles = "admin")]
    [HttpPost("RoleManager/AddRole")]
    public async Task<IActionResult> AddRole(string name)
    {
        var result = await roleManager.CreateAsync(new Role() { Name = name });
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors.ToString());
    }
    [Authorize(Roles = "admin")]
    [HttpDelete("RoleManager/RemoveRole")]
    public async Task<IActionResult> RemoveRole(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role == null)
            return BadRequest("نقش پیدا نشد");
        var result = await roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors.ToString());

    }





    [Authorize]
    [HttpPut("Account/Profile/ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("لطفا اول وارد شوید");
        }

        if (!ModelState.IsValid)
            return BadRequest("مدل ناقص میباشد");
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("عوض کردن رمز موفقیت امیز نبود");
        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (result.Succeeded)
            return Ok();
        return BadRequest(result.Errors.ToString());

    }
    
}