using AutoMapper;
using Core_Layer.Dtos.Comment;
using Data_Layer.Entities;

namespace Core_Layer.Profiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<AddCommentDto, Comment>()
            .ForMember(e => e.UserId, s => s.Ignore())
            .ForMember(e => e.User, s => s.Ignore())
            .ForMember(e => e.Replays, s => s.Ignore());
        CreateMap<Comment, CommentDto>();
    }
}