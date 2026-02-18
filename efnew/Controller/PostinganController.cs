using DTOs.Postingan;
using DTOs.Common;
using Common;
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

        var result = await _postinganService.CreatePostinganAsync(
            dto,
            userId,
            User.Identity?.Name ?? string.Empty);

        return result.ToActionResult(this);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPostingan()
    {
        var result = await _postinganService.GetAllPostinganAsync();
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePostingan(Guid id, UpdatePostinganDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var result = await _postinganService.UpdatePostinganAsync(id, dto, userId);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePostingan(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var result = await _postinganService.DeletePostinganAsync(id, userId);
        return result.ToActionResult(this);
    }
}
