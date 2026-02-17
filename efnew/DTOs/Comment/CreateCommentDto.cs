using System.ComponentModel.DataAnnotations;

namespace DTOs.Comment;

public class CreateCommentDto
{
    [Required]
    public string Isi { get; set; } = string.Empty;
}
