using AutoMapper;
using DTOs.User;
using Common;
using DTOs.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Repositories.Interfaces;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IConfiguration configuration,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<Result<UserResponseDto>> RegisterAsync(RegisterUserDto dto)
    {
        var user = _mapper.Map<Users>(dto);

        var result = await _userRepository.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return Result<UserResponseDto>.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description)),
                400);

        return Result<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto dto)
    {
        var user = await _userRepository.FindByNameAsync(dto.UserName);

        if (user == null || !await _userRepository.CheckPasswordAsync(user, dto.Password))
            return Result<string>.Fail("Invalid credentials", 401);

        return Result<string>.Ok(GenerateJwtToken(user));
    }

    public async Task<Result<List<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return Result<List<UserResponseDto>>.Ok(_mapper.Map<List<UserResponseDto>>(users));
    }

    public async Task<Result<UserResponseDto>> UpdateUserAsync(string userId, UpdateUserDto dto)
    {
        var user = await _userRepository.FindByIdAsync(userId);

        if (user is null)
            return Result<UserResponseDto>.Fail("User tidak ditemukan.", 404);

        _mapper.Map(dto, user);
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userRepository.UpdateAsync(user);

        if (!result.Succeeded)
            return Result<UserResponseDto>.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description)),
                400);

        return Result<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
    }

    public async Task<Result<MessageResponseDto>> DeleteUserAsync(string userId)
    {
        var user = await _userRepository.FindByIdAsync(userId);

        if (user is null)
            return Result<MessageResponseDto>.Fail("User tidak ditemukan.", 404);

        await using var tx = await _userRepository.BeginTransactionAsync();

        var userComments = await _userRepository.GetCommentsByUserIdAsync(userId);

        if (userComments.Count > 0)
            _userRepository.RemoveComments(userComments);

        var userPostingan = await _userRepository.GetPostinganByUserIdAsync(userId);

        if (userPostingan.Count > 0)
            _userRepository.RemovePostingan(userPostingan);

        await _userRepository.SaveChangesAsync();

        var result = await _userRepository.DeleteAsync(user);

        if (!result.Succeeded)
            return Result<MessageResponseDto>.Fail(
                string.Join(", ", result.Errors.Select(e => e.Description)),
                400);

        await tx.CommitAsync();

        return Result<MessageResponseDto>.Ok(new MessageResponseDto
        {
            Message = "User berhasil dihapus."
        });
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
