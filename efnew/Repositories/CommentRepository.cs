using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;

    public CommentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> PostinganExistsAsync(Guid postinganId)
        => _context.Postingan.AnyAsync(p => p.Id == postinganId);

    public Task<List<Comment>> GetByPostinganAsync(Guid postinganId)
        => _context.Comment
            .Where(c => c.PostinganId == postinganId)
            .Include(c => c.User)
            .OrderBy(c => c.Id)
            .ToListAsync();

    public Task AddAsync(Comment comment)
    {
        _context.Comment.Add(comment);
        return Task.CompletedTask;
    }

    public Task<Comment?> GetByIdAsync(Guid postinganId, int commentId)
        => _context.Comment
            .FirstOrDefaultAsync(c => c.Id == commentId && c.PostinganId == postinganId);

    public void Remove(Comment comment)
        => _context.Comment.Remove(comment);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
