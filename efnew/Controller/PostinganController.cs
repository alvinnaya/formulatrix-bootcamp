using DTOs.Postingan;
using DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostinganController : ControllerBase
{
    private readonly IPostinganService _postinganService;

    public PostinganController(IPostinganService postinganService)
    {
        _postinganService = postinganService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePostingan(CreatePostinganDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var response = await _postinganService.CreatePostinganAsync(
            dto,
            userId,
            User.Identity?.Name ?? string.Empty);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostingan()
    {
        var posts = await _postinganService.GetAllPostinganAsync();
        return Ok(posts);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePostingan(Guid id, UpdatePostinganDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        try
        {
            var response = await _postinganService.UpdatePostinganAsync(id, dto, userId);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePostingan(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        try
        {
            await _postinganService.DeletePostinganAsync(id, userId);
            return Ok(new MessageResponseDto { Message = "Postingan berhasil dihapus." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
