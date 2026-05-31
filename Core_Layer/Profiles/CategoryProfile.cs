using AutoMapper;
using Core_Layer.Dtos.Category;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<AddAndUpdateCategoryDto, Category>()
            .ForMember(e => e.Slug, s => s.Ignore())
            .ForMember(e => e.Id, s => s.Ignore()); ;

    }
}