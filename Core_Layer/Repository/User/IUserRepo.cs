using Core_Layer.Dtos.AccountDto;

namespace Core_Layer.Repository.User;

public interface IUserRepo
{
    public Task<List<UserDto>> GetUsersWithRoles();
}