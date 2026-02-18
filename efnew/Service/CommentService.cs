using AutoMapper;
using DTOs.Comment;
using Common;
using DTOs.Common;
using Models;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;

    public CommentService(ICommentRepository commentRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<CommentResponseDto>>> GetCommentsByPostinganAsync(Guid postinganId)
    {
        var postinganExists = await _commentRepository.PostinganExistsAsync(postinganId);

        if (!postinganExists)
            return Result<List<CommentResponseDto>>.Fail("Postingan tidak ditemukan.", 404);

        var comments = await _commentRepository.GetByPostinganAsync(postinganId);
        return Result<List<CommentResponseDto>>.Ok(_mapper.Map<List<CommentResponseDto>>(comments));
    }

    public async Task<Result<CommentResponseDto>> CreateCommentAsync(Guid postinganId, CreateCommentDto dto, string userId, string userName)
    {
        var postinganExists = await _commentRepository.PostinganExistsAsync(postinganId);

        if (!postinganExists)
            return Result<CommentResponseDto>.Fail("Postingan tidak ditemukan.", 404);

        var comment = _mapper.Map<Comment>(dto);
        comment.PostinganId = postinganId;
        comment.UserId = userId;

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var response = _mapper.Map<CommentResponseDto>(comment);
        response.UserName = userName;
        return Result<CommentResponseDto>.Ok(response);
    }

    public async Task<Result<MessageResponseDto>> DeleteCommentAsync(Guid postinganId, int commentId, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(postinganId, commentId);

        if (comment is null)
            return Result<MessageResponseDto>.Fail("Comment tidak ditemukan.", 404);

        if (comment.UserId != userId)
            return Result<MessageResponseDto>.Fail("Anda tidak memiliki akses.", 403);

        _commentRepository.Remove(comment);
        await _commentRepository.SaveChangesAsync();

        return Result<MessageResponseDto>.Ok(new MessageResponseDto
        {
            Message = "Comment berhasil dihapus."
        });
    }
}
