namespace Core_Layer.Dtos.AccountDto;

public class LoginUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsPersistent { get; set; } = false;
}