using AutoMapper;
using Core_Layer.Dtos.ImageDto;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<Image, ImageDto>()
            .ForMember(e=>e.ImageName,s=>s.MapFrom(e=>e.Name));

    }
}