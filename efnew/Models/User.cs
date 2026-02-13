using Microsoft.AspNetCore.Identity;

namespace Models;



public class Users : IdentityUser
{
    public String Name {get; set;}
   
    public ICollection<Postingan> Postingan { get; set; } = new List<Postingan>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<Comment> Comments { get; set; } = new();

}
