using Busines_Layer.Dtos.AccountDto;

namespace Busines_Layer.Repository.User;

public interface IUserRepo
{
    public Task<List<UserDto>> GetUsersWithRoles();
}