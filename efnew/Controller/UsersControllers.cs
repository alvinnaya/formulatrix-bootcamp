using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("getalluser")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.CreatedAt,
                u.UpdatedAt,
                TotalPostingan = u.Postingan.Count
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Postingan)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new
        {
            user.Id,
            user.Name,
            user.CreatedAt,
            user.UpdatedAt,
            Postingan = user.Postingan.Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.UserId,
                p.CreatedAt,
                p.UpdatedAt
            })
        });
    }

    [HttpPost("createUser")]
    public async Task<ActionResult<Users>> Create(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Name is required" });

        var user = new Users
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new
        {
            user.Id,
            user.Name,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    [HttpGet("{userId}/postingan")]
    public async Task<IActionResult> GetAllPostinganByUser(Guid userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return NotFound(new { message = "User not found" });

        var postingan = await _context.Postingan
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.UserId,
                p.CreatedAt,
                p.UpdatedAt
            })
            .ToListAsync();

        return Ok(postingan);
    }

    [HttpPost("{userId}/postingan")]
    public async Task<IActionResult> CreatePostingan(Guid userId, CreatePostinganRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required" });

        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            return NotFound(new { message = "User not found" });

        var postingan = new Postingan
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Content = request.Content?.Trim() ?? string.Empty,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Postingan.Add(postingan);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllPostinganByUser), new { userId }, new
        {
            postingan.Id,
            postingan.Title,
            postingan.Content,
            postingan.UserId,
            postingan.CreatedAt,
            postingan.UpdatedAt
        });
    }

    [HttpPut("{userId}/postingan/{postinganId}")]
    public async Task<IActionResult> UpdatePostingan(Guid userId, Guid postinganId, UpdatePostinganRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required" });

        var existingPostingan = await _context.Postingan
            .FirstOrDefaultAsync(p => p.Id == postinganId && p.UserId == userId);

        if (existingPostingan == null)
            return NotFound(new { message = "Postingan not found for this user" });

        existingPostingan.Title = request.Title.Trim();
        existingPostingan.Content = request.Content?.Trim() ?? string.Empty;
        existingPostingan.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            existingPostingan.Id,
            existingPostingan.Title,
            existingPostingan.Content,
            existingPostingan.UserId,
            existingPostingan.CreatedAt,
            existingPostingan.UpdatedAt
        });
    }

    [HttpDelete("{userId}/postingan/{postinganId}")]
    public async Task<IActionResult> DeletePostingan(Guid userId, Guid postinganId)
    {
        var postingan = await _context.Postingan
            .FirstOrDefaultAsync(p => p.Id == postinganId && p.UserId == userId);

        if (postingan == null)
            return NotFound(new { message = "Postingan not found for this user" });

        _context.Postingan.Remove(postingan);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Postingan deleted successfully" });
    }

    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CreatePostinganRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }

    public class UpdatePostinganRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
    }
}
