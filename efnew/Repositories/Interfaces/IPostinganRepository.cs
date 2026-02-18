using Models;

namespace Repositories.Interfaces;

public interface IPostinganRepository
{
    Task AddAsync(Postingan post);
    Task<List<Postingan>> GetAllWithUserAsync();
    Task<Postingan?> GetByIdWithUserAsync(Guid id);
    Task<Postingan?> GetByIdAsync(Guid id);
    void Remove(Postingan post);
    Task SaveChangesAsync();
}
