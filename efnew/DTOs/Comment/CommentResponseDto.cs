namespace DTOs.Comment;

public class CommentResponseDto
{
    public int Id { get; set; }
    public string Isi { get; set; } = string.Empty;
    public Guid PostinganId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
