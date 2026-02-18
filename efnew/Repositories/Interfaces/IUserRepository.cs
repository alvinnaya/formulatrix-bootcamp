using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Models;

namespace Repositories.Interfaces;

public interface IUserRepository
{
    Task<Users?> FindByIdAsync(string userId);
    Task<Users?> FindByNameAsync(string userName);
    Task<List<Users>> GetAllAsync();
    Task<IdentityResult> CreateAsync(Users user, string password);
    Task<bool> CheckPasswordAsync(Users user, string password);
    Task<IdentityResult> UpdateAsync(Users user);
    Task<IdentityResult> DeleteAsync(Users user);
    Task<List<Comment>> GetCommentsByUserIdAsync(string userId);
    Task<List<Postingan>> GetPostinganByUserIdAsync(string userId);
    void RemoveComments(IEnumerable<Comment> comments);
    void RemovePostingan(IEnumerable<Postingan> postingan);
    Task SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
