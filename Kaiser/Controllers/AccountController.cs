
using Core_Layer.Dtos.AccountDto;
using Core_Layer.Repository.User;
using Core_Layer.Services.Ghasedak;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Kaiser.Controllers;

[ApiController]
public class AccountController(UserManager<User> userManager,
    SignInManager<User> signInManager,
    RoleManager<Role> roleManager,
    IUserRepo userRepo,
    ISmsServices smsServices
    ) : ControllerBase
{

    [EnableRateLimiting("login")]
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
    [EnableRateLimiting("login")]
    [HttpPost("LoginWithPhone")]
    public async Task<IActionResult> LoginWithPhone(string phone)
    {
        if (phone.Length != 11)
        {
            return BadRequest("فرمت شماره تماس نادرست است");
        }
        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phone);

        if (user == null)
        {
            user = new User
            {
                UserName = phone,
                PhoneNumber = phone,
                PhoneNumberConfirmed = false
            };

            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);
        }

        var code = await userManager.GenerateChangePhoneNumberTokenAsync(
            user,
            phone);

        await smsServices.SendOtpAsync(phone, "Ghasedak", code);

        return Ok(phone);
    }
    [HttpPost("VerifyPhone")]
    public async Task<IActionResult> VerifyPhone(string phone,string code)
    {
        if (phone.Length != 11)
        {
            return BadRequest("فرمت شماره تماس نادرست است");
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phone);

        if (user == null)
            return BadRequest("کاربر یافت نشد.");

        var valid = await userManager.VerifyChangePhoneNumberTokenAsync(
            user,
            code,
            phone);

        if (!valid)
            return BadRequest("کد نامعتبر است.");

        if (!user.PhoneNumberConfirmed)
        {
            user.PhoneNumberConfirmed = true;
            await userManager.UpdateAsync(user);
        }

        await signInManager.SignInAsync(user, isPersistent: false);

        return Ok();
    }
    [EnableRateLimiting("login")]
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
        {

            await userManager.UpdateSecurityStampAsync(user);
            return Ok();
        }
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
        {
            
            await userManager.UpdateSecurityStampAsync(user);
            return Ok();
        }
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