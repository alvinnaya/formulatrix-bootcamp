using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories;

public class PostinganRepository : IPostinganRepository
{
    private readonly AppDbContext _context;

    public PostinganRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Postingan post)
    {
        _context.Postingan.Add(post);
        return Task.CompletedTask;
    }

    public Task<List<Postingan>> GetAllWithUserAsync()
        => _context.Postingan
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public Task<Postingan?> GetByIdWithUserAsync(Guid id)
        => _context.Postingan
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

    public Task<Postingan?> GetByIdAsync(Guid id)
        => _context.Postingan.FirstOrDefaultAsync(p => p.Id == id);

    public void Remove(Postingan post)
        => _context.Postingan.Remove(post);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
