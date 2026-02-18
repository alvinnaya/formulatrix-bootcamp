using Common;
using DTOs.Comment;
using DTOs.Common;

namespace Services.Interfaces;

public interface ICommentService
{
    Task<Result<List<CommentResponseDto>>> GetCommentsByPostinganAsync(Guid postinganId);
    Task<Result<CommentResponseDto>> CreateCommentAsync(Guid postinganId, CreateCommentDto dto, string userId, string userName);
    Task<Result<MessageResponseDto>> DeleteCommentAsync(Guid postinganId, int commentId, string userId);
}
