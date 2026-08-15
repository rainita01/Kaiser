namespace Busines_Layer.Dtos.AccountDto;

public class RegisterUserDto
{
    public string Username { get; set; }    
    public string Password { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string? PhoneNumber { get; set; }    

}