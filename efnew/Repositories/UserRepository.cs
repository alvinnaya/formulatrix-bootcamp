using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models;
using Repositories.Interfaces;

namespace Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<Users> _userManager;
    private readonly AppDbContext _context;

    public UserRepository(UserManager<Users> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public Task<Users?> FindByIdAsync(string userId)
        => _userManager.FindByIdAsync(userId);

    public Task<Users?> FindByNameAsync(string userName)
        => _userManager.FindByNameAsync(userName);

    public Task<List<Users>> GetAllAsync()
        => _userManager.Users.ToListAsync();

    public Task<IdentityResult> CreateAsync(Users user, string password)
        => _userManager.CreateAsync(user, password);

    public Task<bool> CheckPasswordAsync(Users user, string password)
        => _userManager.CheckPasswordAsync(user, password);

    public Task<IdentityResult> UpdateAsync(Users user)
        => _userManager.UpdateAsync(user);

    public Task<IdentityResult> DeleteAsync(Users user)
        => _userManager.DeleteAsync(user);

    public Task<List<Comment>> GetCommentsByUserIdAsync(string userId)
        => _context.Comment.Where(c => c.UserId == userId).ToListAsync();

    public Task<List<Postingan>> GetPostinganByUserIdAsync(string userId)
        => _context.Postingan.Where(p => p.UserId == userId).ToListAsync();

    public void RemoveComments(IEnumerable<Comment> comments)
        => _context.Comment.RemoveRange(comments);

    public void RemovePostingan(IEnumerable<Postingan> postingan)
        => _context.Postingan.RemoveRange(postingan);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync()
        => _context.Database.BeginTransactionAsync();
}
