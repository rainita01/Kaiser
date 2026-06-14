using AutoMapper;
using Core_Layer.Dtos.AddressDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class AddressProfile : Profile
{
    public AddressProfile()
    {
        CreateMap<AddAddressDto, Address>()
            .ForMember(e => e.Id, s => s.Ignore())
            .ForMember(e => e.User, s => s.Ignore());
        CreateMap<Address, AddressDto>();
    }
}