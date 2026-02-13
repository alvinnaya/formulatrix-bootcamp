using DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class UserService : IUserService
{
    private readonly UserManager<Users> _userManager;
    private readonly IConfiguration _configuration;

    public UserService(UserManager<Users> userManager,
                       IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        var user = new Users
        {
            UserName = dto.UserName,
            Name = dto.Name
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            

        return new UserResponseDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Name = user.Name
        };
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new Exception("Invalid credentials");

        Console.WriteLine(user.Id);
        Console.WriteLine(user.UserName);
        Console.WriteLine(user.Name);


        return GenerateJwtToken(user);
    }

  public async Task<List<UserResponseDto>> GetAllUsersAsync()
{
    var users = _userManager.Users.ToList();

    var result = users.Select(user => new UserResponseDto
    {
        Id = user.Id,
        UserName = user.UserName!,
        Name = user.Name
    }).ToList();

    return await Task.FromResult(result);
}


    private string GenerateJwtToken(Users user)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["Key"]!)
        );

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
           
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
