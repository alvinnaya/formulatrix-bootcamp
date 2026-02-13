using DTOs.User;

namespace Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<string> LoginAsync(LoginUserDto dto);
}
