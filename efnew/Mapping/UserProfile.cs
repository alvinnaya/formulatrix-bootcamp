using AutoMapper;
using DTOs.User;
using Models;

namespace Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterUserDto, Users>();
        CreateMap<UpdateUserDto, Users>();
        CreateMap<Users, UserResponseDto>();
    }
}
