using AutoMapper;
using Core_Layer.Dtos.Product;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<AddProductDto, Product>()
            .ForMember(e => e.Images, s => s.Ignore())
            .ForMember(e=>e.Slug,s=>s.Ignore());
        CreateMap<Product, ProductCardDto>()
            .ForMember(e => e.Image, s => s.MapFrom(e=>e.Images != null ? e.Images.First().Name : ""))
            .ForMember(e=>e.Views ,s=>s.MapFrom(p => p.ProductViews != null ? (long)p.ProductViews.Count() : 0));
        CreateMap<Product, UpdateProductDto>();
        CreateMap<Product, ProductDto>()
            .ForMember(e => e.Images, s => s.Ignore())
            .ForMember(e => e.Views, s => s.Ignore());

    }
}