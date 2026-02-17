using System.ComponentModel.DataAnnotations;

namespace DTOs.Postingan;

public class CreatePostinganDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
