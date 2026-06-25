namespace Core_Layer.Dtos.AccountDto;

public class RolesWithUsers
{
    public string RoleName { get; set; }
    public List<UserNameWithIdDto> Users { get; set; }
}