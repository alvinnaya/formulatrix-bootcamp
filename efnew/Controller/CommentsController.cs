using DTOs.Comment;
using DTOs.Common;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Controllers;

[ApiController]
[Route("api/postingan/{postinganId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentsByPostingan(Guid postinganId)
    {
        var result = await _commentService.GetCommentsByPostinganAsync(postinganId);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateComment(Guid postinganId, CreateCommentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var result = await _commentService.CreateCommentAsync(
            postinganId,
            dto,
            userId,
            User.Identity?.Name ?? string.Empty);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpDelete("{commentId:int}")]
    public async Task<IActionResult> DeleteComment(Guid postinganId, int commentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Token tidak valid.");

        var result = await _commentService.DeleteCommentAsync(postinganId, commentId, userId);
        return result.ToActionResult(this);
    }
}
