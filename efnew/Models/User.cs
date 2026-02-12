namespace Models;

public class Users
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public ICollection<Postingan> Postingan { get; set; } = new List<Postingan>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; }
}
