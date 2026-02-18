using Models;

namespace Repositories.Interfaces;

public interface ICommentRepository
{
    Task<bool> PostinganExistsAsync(Guid postinganId);
    Task<List<Comment>> GetByPostinganAsync(Guid postinganId);
    Task AddAsync(Comment comment);
    Task<Comment?> GetByIdAsync(Guid postinganId, int commentId);
    void Remove(Comment comment);
    Task SaveChangesAsync();
}
