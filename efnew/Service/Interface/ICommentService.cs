using DTOs.Comment;

namespace Services.Interfaces;

public interface ICommentService
{
    Task<List<CommentResponseDto>> GetCommentsByPostinganAsync(Guid postinganId);
    Task<CommentResponseDto> CreateCommentAsync(Guid postinganId, CreateCommentDto dto, string userId, string userName);
    Task DeleteCommentAsync(Guid postinganId, int commentId, string userId);
}
