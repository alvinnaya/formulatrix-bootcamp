using Models;

public class Comment
{
    public int Id { get; set; }
    public string Isi { get; set; } 

    // Foreign key ke User
    public string UserId { get; set; }
    public Users User { get; set; }

    // Foreign key ke Postingan
    public Guid PostinganId { get; set; }
    public Postingan Postingan { get; set; } 
}