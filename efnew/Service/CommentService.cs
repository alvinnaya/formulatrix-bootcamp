using AutoMapper;
using Data;
using DTOs.Comment;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;

namespace Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CommentService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CommentResponseDto>> GetCommentsByPostinganAsync(Guid postinganId)
    {
        var postinganExists = await _context.Postingan.AnyAsync(p => p.Id == postinganId);

        if (!postinganExists)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        var comments = await _context.Comment
            .Where(c => c.PostinganId == postinganId)
            .Include(c => c.User)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return _mapper.Map<List<CommentResponseDto>>(comments);
    }

    public async Task<CommentResponseDto> CreateCommentAsync(Guid postinganId, CreateCommentDto dto, string userId, string userName)
    {
        var postinganExists = await _context.Postingan.AnyAsync(p => p.Id == postinganId);

        if (!postinganExists)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        var comment = _mapper.Map<Comment>(dto);
        comment.PostinganId = postinganId;
        comment.UserId = userId;

        _context.Comment.Add(comment);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<CommentResponseDto>(comment);
        response.UserName = userName;
        return response;
    }

    public async Task DeleteCommentAsync(Guid postinganId, int commentId, string userId)
    {
        var comment = await _context.Comment
            .FirstOrDefaultAsync(c => c.Id == commentId && c.PostinganId == postinganId);

        if (comment is null)
            throw new KeyNotFoundException("Comment tidak ditemukan.");

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _context.Comment.Remove(comment);
        await _context.SaveChangesAsync();
    }
}
