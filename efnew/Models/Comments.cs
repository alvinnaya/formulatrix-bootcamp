using Models;

public class Comment
{
    public int Id { get; set; }
    public string Isi { get; set; }

    // Foreign key ke User
    public int UserId { get; set; }
    public Users User { get; set; }

    // Foreign key ke Postingan
    public int PostinganId { get; set; }
    public Postingan Postingan { get; set; }
}