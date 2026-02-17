using AutoMapper;
using DTOs.Postingan;
using Models;

namespace Mapping;

public class PostinganProfile : Profile
{
    public PostinganProfile()
    {
        CreateMap<CreatePostinganDto, Postingan>();
        CreateMap<UpdatePostinganDto, Postingan>();

        CreateMap<Postingan, PostinganResponseDto>()
            .ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName ?? string.Empty : string.Empty)
            );
    }
}
