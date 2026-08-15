using AutoMapper;
using Busines_Layer.Dtos.CartDto;
using Busines_Layer.Dtos.SnapShotDto;
using Data_Layer.Entities;

namespace Busines_Layer.Profiles;

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
            .ForMember(e=>e.ProductName,s=>s.MapFrom(e=>e.Product.Name))
            .ForMember(e=>e.SnapShotId,s=>s.Ignore());
        CreateMap<CartItem,SnapShotItem>()
            .ForMember(e => e.UnitPrice, s => s.MapFrom(e => e.Product.Price))
            .ForMember(e => e.Count, s => s.MapFrom(e => e.Quantity))
            .ForMember(e => e.Discount, s => s.MapFrom(e => e.Product.DiscountPercent))
            .ForMember(e => e.Id, s => s.Ignore())
            .ForMember(e => e.ProductId, s => s.MapFrom(e => e.ProductId))
            .ForMember(e => e.ProductName, s => s.MapFrom(e => e.Product.Name))
            .ForMember(e => e.SnapShotId, s => s.Ignore());
        CreateMap<CartItemDto,SnapShotItem>().ForMember(e => e.UnitPrice, s => s.MapFrom(e => e.Price))
            .ForMember(e => e.Count, s => s.MapFrom(e => e.Quantity))
            .ForMember(e => e.Discount, s => s.MapFrom(e => e.DiscountPercent))
            .ForMember(e => e.Id, s => s.Ignore())
            .ForMember(e => e.ProductId, s => s.MapFrom(e => e.ProductId))
            .ForMember(e => e.ProductName, s => s.MapFrom(e => e.Name))
            .ForMember(e => e.SnapShotId, s => s.Ignore());

        CreateMap<SnapShotDto, SnapShot>()
            .ForMember(e => e.User, s => s.Ignore());
        CreateMap<SnapShotItemDto, SnapShotItem>()
            .ForMember(e => e.SnapShot, s => s.Ignore());
    }
}