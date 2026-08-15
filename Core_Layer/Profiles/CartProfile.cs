using AutoMapper;
using Busines_Layer.Dtos.CartDto;
using Busines_Layer.Dtos.Postex;

namespace Busines_Layer.Profiles;

public class CartProfile : Profile
{
    public CartProfile()
    {

        CreateMap<CartItemDto, ParcelPropertyDto>()
            .ForMember(e => e.TotalValue, s => s.MapFrom(a => a.Price));
    }
}