using Core_Layer.Dtos.AccountDto;
using Data_Layer.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Repository.User;

public class UserRepo(Context context) : IUserRepo
{
    public async Task<List<UserDto>> GetUsersWithRoles()
    {
        var query = from user in context.Users
            join userRole in context.UserRoles
                on user.Id equals userRole.UserId into userRolesGroup
            select new UserDto()
            {
                Id = user.Id,
                Username = user.UserName,
                Firstname = user.FirstName,
                Lastname = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = (from ur in userRolesGroup
                    join role in context.Roles
                        on ur.RoleId equals role.Id
                    select role.Name).ToList()
            };


        return await query.ToListAsync();
    }
}