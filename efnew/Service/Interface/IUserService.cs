using DTOs.User;

namespace Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(RegisterUserDto dto);
    Task<string> LoginAsync(LoginUserDto dto);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto);
    Task DeleteUserAsync(string userId);
}
