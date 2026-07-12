using AutoMapper;
using Core_Layer.Dtos.AddressDto;
using Data_Layer.Entities;
using Microsoft.AspNetCore.Components;

namespace Core_Layer.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddAddressDto, Address>()
            .ForMember(e => e.Id, s => s.Ignore())
            .ForMember(e => e.User, s => s.Ignore());
        CreateMap<Address, AddressDto>()
            .ForMember(e=>e.City,s=>s.MapFrom(e=>e.City.Name))
            .ForMember(e => e.Province, s => s.MapFrom(e => e.Province.Name));

        CreateMap<Province, ProviceDto>();
        CreateMap<City, CityDto>();
    }
}