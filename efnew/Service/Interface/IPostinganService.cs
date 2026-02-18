using Common;
using DTOs.Common;
using DTOs.Postingan;

namespace Services.Interfaces;

public interface IPostinganService
{
    Task<Result<PostinganResponseDto>> CreatePostinganAsync(CreatePostinganDto dto, string userId, string userName);
    Task<Result<List<PostinganResponseDto>>> GetAllPostinganAsync();
    Task<Result<PostinganResponseDto>> UpdatePostinganAsync(Guid id, UpdatePostinganDto dto, string userId);
    Task<Result<MessageResponseDto>> DeletePostinganAsync(Guid id, string userId);
}
