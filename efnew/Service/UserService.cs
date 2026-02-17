using AutoMapper;
using DTOs.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    private readonly IMapper _mapper;

    public UserService(
        UserManager<Users> userManager,
        IConfiguration configuration,
        IMapper mapper)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        var user = _mapper.Map<Users>(dto);

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new Exception("Invalid credentials");

        return GenerateJwtToken(user);
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return _mapper.Map<List<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new Exception("User tidak ditemukan.");

        _mapper.Map(dto, user);
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new Exception("User tidak ditemukan.");

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
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
