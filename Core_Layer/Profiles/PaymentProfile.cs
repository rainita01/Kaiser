using AutoMapper;
using Busines_Layer.Dtos.PaymentDto;
using Data_Layer.Entities;

namespace Busines_Layer.Profiles;

public class PaymentProfile :Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentDto>();
    }
}