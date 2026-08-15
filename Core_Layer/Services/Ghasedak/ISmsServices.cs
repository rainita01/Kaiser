namespace Busines_Layer.Services.Ghasedak;

public interface ISmsServices
{
    Task SendAsync(string phoneNumber, string message);

    Task SendOtpAsync(string phoneNumber, string template, params string[] parameters);
}