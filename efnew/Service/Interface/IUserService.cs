using Common;
using DTOs.Common;
using DTOs.User;

namespace Services.Interfaces;

public interface IUserService
{
    Task<Result<UserResponseDto>> RegisterAsync(RegisterUserDto dto);
    Task<Result<string>> LoginAsync(LoginUserDto dto);
    Task<Result<List<UserResponseDto>>> GetAllUsersAsync();
    Task<Result<UserResponseDto>> UpdateUserAsync(string userId, UpdateUserDto dto);
    Task<Result<MessageResponseDto>> DeleteUserAsync(string userId);
}
