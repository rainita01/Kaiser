using AutoMapper;
using Core_Layer.Dtos.PaymentDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class PaymentProfile :Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, PaymentDto>();
    }
}