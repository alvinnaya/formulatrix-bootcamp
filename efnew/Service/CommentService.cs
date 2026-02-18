using AutoMapper;
using DTOs.Comment;
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

    public async Task<List<CommentResponseDto>> GetCommentsByPostinganAsync(Guid postinganId)
    {
        var postinganExists = await _commentRepository.PostinganExistsAsync(postinganId);

        if (!postinganExists)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        var comments = await _commentRepository.GetByPostinganAsync(postinganId);
        return _mapper.Map<List<CommentResponseDto>>(comments);
    }

    public async Task<CommentResponseDto> CreateCommentAsync(Guid postinganId, CreateCommentDto dto, string userId, string userName)
    {
        var postinganExists = await _commentRepository.PostinganExistsAsync(postinganId);

        if (!postinganExists)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        var comment = _mapper.Map<Comment>(dto);
        comment.PostinganId = postinganId;
        comment.UserId = userId;

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        var response = _mapper.Map<CommentResponseDto>(comment);
        response.UserName = userName;
        return response;
    }

    public async Task DeleteCommentAsync(Guid postinganId, int commentId, string userId)
    {
        var comment = await _commentRepository.GetByIdAsync(postinganId, commentId);

        if (comment is null)
            throw new KeyNotFoundException("Comment tidak ditemukan.");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _commentRepository.Remove(comment);
        await _commentRepository.SaveChangesAsync();
    }
}
