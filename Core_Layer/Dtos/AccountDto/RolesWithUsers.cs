namespace Busines_Layer.Dtos.AccountDto;

public class RolesWithUsersDto
{
    public string RoleName { get; set; }
    public List<UserNameWithIdDto> Users { get; set; }
}