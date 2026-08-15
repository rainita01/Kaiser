using AutoMapper;
using Busines_Layer.Dtos.Category;
using Data_Layer.Entities;

namespace Busines_Layer.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<AddCategoryDto, Category>()
            .ForMember(e => e.Slug, s => s.Ignore())
            .ForMember(e => e.Id, s => s.Ignore()); ;

    }
}