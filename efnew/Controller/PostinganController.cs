using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Controller;

[ApiController]
[Route("api/[controller]")]
public class PostinganController : ControllerBase
{
    private readonly AppDbContext _context;

    public PostinganController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostingan([FromQuery] Guid? userId)
    {
        var query = _context.Postingan
            .AsNoTracking()
            .Include(p => p.User)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        var postingan = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.UserId,
                UserName = p.User != null ? p.User.Name : null,
                p.CreatedAt,
                p.UpdatedAt
            })
            .ToListAsync();

        return Ok(postingan);
    }

    [HttpGet("{postinganId}")]
    public async Task<IActionResult> GetPostinganById(Guid postinganId)
    {
        var postingan = await _context.Postingan
            .AsNoTracking()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == postinganId);

        if (postingan == null)
            return NotFound(new { message = "Postingan not found" });

        return Ok(new
        {
            postingan.Id,
            postingan.Title,
            postingan.Content,
            postingan.UserId,
            UserName = postingan.User != null ? postingan.User.Name : null,
            postingan.CreatedAt,
            postingan.UpdatedAt
        });
    }

    [HttpGet("{postinganId}/comments")]
    public async Task<IActionResult> GetCommentsByPostingan(Guid postinganId)
    {
        var postingan = await _context.Postingan
            .AsNoTracking()
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postinganId);

        if (postingan == null)
            return NotFound(new { message = "Postingan not found" });

        var comments = postingan.Comments
            .Select(c => new
            {
                c.Id,
                c.Isi,
                c.UserId,
                c.PostinganId
            })
            .ToList();

        return Ok(comments);
    }
}
