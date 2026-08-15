using GhasedakSms.Core.Dto;
using Microsoft.Extensions.Options;
namespace Busines_Layer.Services.Ghasedak;
public class GhasedakSmsService : ISmsServices
{
    private readonly GhasedakSms.Core.Ghasedak _client;
    private readonly GhasedakOption _options;

    public GhasedakSmsService(IOptions<GhasedakOption> options)
    {
        _options = options.Value;

        _client = new GhasedakSms.Core.Ghasedak(_options.ApiKey);
    }
    public async Task SendAsync(string phoneNumber, string message)
    {
        SendSingleSmsInput command = new SendSingleSmsInput()
        {
            Receptor = phoneNumber,
            Message = message,
            LineNumber = _options.LineNumber
        };
        await _client.SendSingleSMS(command, CancellationToken.None);
    }

    public async Task SendOtpAsync(string phoneNumber, string template, params string[] parameters)
    {
        if (parameters.Length == 0)
            throw new ArgumentException("OTP parameter is required.");

        await _client.SendOtpSMS(new SendOtpInput()
        {
            TemplateName = template,
            SendDate = DateTime.Now,
            Inputs = new List<OtpInput>()
            {
                new OtpInput(){Param="Code", Value=parameters[0]},
            },
            Receptors = new List<SendOtpReceptorDto>() {
                new SendOtpReceptorDto() {ClientReferenceId = "testOtp1", Mobile = phoneNumber},
            },
        });
    }
}