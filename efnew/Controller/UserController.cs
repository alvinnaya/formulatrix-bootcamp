using Data;
using DTOs.Common;
using DTOs.Postingan;
using DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interfaces;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public UsersController(IUserService userService, AppDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var token = await _userService.LoginAsync(dto);
        return Ok(new { token });
    }

    [HttpGet("getallUser")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [Authorize]
    [HttpPut("edit")]
    public async Task<IActionResult> UpdateMe(UpdateUserDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        try
        {
            var response = await _userService.UpdateUserAsync(userId, dto);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        try
        {
            await _userService.DeleteUserAsync(userId);
            return Ok(new MessageResponseDto { Message = "User berhasil dihapus." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("postingan")]
    public async Task<IActionResult> CreatePostingan(CreatePostinganDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var post = new Postingan
        {
            Title = dto.Title,
            Content = dto.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Postingan.Add(post);
        await _context.SaveChangesAsync();

        var response = new PostinganResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            UserId = post.UserId,
            UserName = User.Identity?.Name ?? string.Empty,
            CreatedAt = post.CreatedAt
        };

        return Ok(response);
    }
}
