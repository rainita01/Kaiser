using AutoMapper;
using Core_Layer.Dtos.OrderDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<SnapShot, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SnapShotId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SnapShot, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentId, opt => opt.Ignore())
            .ForMember(dest => dest.Payment, opt => opt.Ignore())
            .ForMember(dest => dest.State, opt => opt.MapFrom(src => OrderState.Paid))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));


        CreateMap<SnapShotItem, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());
        CreateMap<AddOrderDto, Order>();
        CreateMap<AddOrderItemDto, OrderItemDto>();
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.TotalPrice,
                opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.Items));
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.Quantity,
                opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.TotalPrice,
                opt => opt.MapFrom(src =>
                    (long)(src.UnitPrice * (1 - src.Discount / 100m) * src.Count)));
    }

}