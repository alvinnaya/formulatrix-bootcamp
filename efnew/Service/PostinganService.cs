using AutoMapper;
using Data;
using DTOs.Postingan;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Interfaces;

namespace Services;

public class PostinganService : IPostinganService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PostinganService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PostinganResponseDto> CreatePostinganAsync(CreatePostinganDto dto, string userId, string userName)
    {
        var post = _mapper.Map<Postingan>(dto);
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;

        _context.Postingan.Add(post);
        await _context.SaveChangesAsync();

        var response = _mapper.Map<PostinganResponseDto>(post);
        response.UserName = userName;

        return response;
    }

    public async Task<List<PostinganResponseDto>> GetAllPostinganAsync()
    {
        var posts = await _context.Postingan
            .Include(p => p.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<PostinganResponseDto>>(posts);
    }

    public async Task<PostinganResponseDto> UpdatePostinganAsync(Guid id, UpdatePostinganDto dto, string userId)
    {
        var post = await _context.Postingan
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _mapper.Map(dto, post);
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<PostinganResponseDto>(post);
    }

    public async Task DeletePostinganAsync(Guid id, string userId)
    {
        var post = await _context.Postingan.FirstOrDefaultAsync(p => p.Id == id);

        if (post is null)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _context.Postingan.Remove(post);
        await _context.SaveChangesAsync();
    }
}
