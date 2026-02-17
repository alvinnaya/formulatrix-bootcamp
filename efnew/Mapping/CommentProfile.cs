using AutoMapper;
using DTOs.Comment;

namespace Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDto, Comment>();
        CreateMap<Comment, CommentResponseDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName ?? string.Empty : string.Empty)
            );
    }
}
