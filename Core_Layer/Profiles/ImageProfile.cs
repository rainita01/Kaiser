using AutoMapper;
using Busines_Layer.Dtos.ImageDto;
using Data_Layer.Entities;

namespace Busines_Layer.Profiles;

public class ImageProfile : Profile
{
    public ImageProfile()
    {
        CreateMap<Image, ImageDto>()
            .ForMember(e=>e.ImageName,s=>s.MapFrom(e=>e.Name));

    }
}