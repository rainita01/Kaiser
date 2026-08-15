using AutoMapper;
using Busines_Layer.Dtos.ContactUsDto;
using Data_Layer.Entities;

namespace Busines_Layer.Profiles;

public class ContactUsProfile : Profile
{
    public ContactUsProfile()
    {
        CreateMap<ContactMessage, ContactUsDto>().ReverseMap();
    }
}