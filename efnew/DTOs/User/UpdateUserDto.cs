using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UpdateUserDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
}
