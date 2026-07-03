using AutoMapper;
using Core_Layer.Dtos.SnapShotDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class CartAndSnapShotProfile : Profile
{
    public CartAndSnapShotProfile()
    {
        CreateMap<CartItem, SnapShotItemDto>()
            .ForMember(e => e.UnitPrice, s => s.MapFrom(e => e.Product.Price))
            .ForMember(e=>e.Count ,s=>s.MapFrom(e=>e.Quantity))
            .ForMember(e=>e.Discount,s=>s.MapFrom(e=>e.Product.DiscountPercent))
            .ForMember(e=>e.Id,s=>s.Ignore())
            .ForMember(e=>e.ProductId,s=>s.MapFrom(e=>e.ProductId))
            .ForMember(e=>e.ProductName,s=>s.MapFrom(e=>e.Product))
            .ForMember(e=>e.SnapShotId,s=>s.Ignore());

    }
}