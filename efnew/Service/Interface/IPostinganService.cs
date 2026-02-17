using DTOs.Postingan;

namespace Services.Interfaces;

public interface IPostinganService
{
    Task<PostinganResponseDto> CreatePostinganAsync(CreatePostinganDto dto, string userId, string userName);
    Task<List<PostinganResponseDto>> GetAllPostinganAsync();
    Task<PostinganResponseDto> UpdatePostinganAsync(Guid id, UpdatePostinganDto dto, string userId);
    Task DeletePostinganAsync(Guid id, string userId);
}
