using AutoMapper;
using Core_Layer.Dtos.ContactUsDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class ContactUsProfile : Profile
{
    public ContactUsProfile()
    {
        CreateMap<ContactMessage, ContactUsDto>().ReverseMap();
    }
}