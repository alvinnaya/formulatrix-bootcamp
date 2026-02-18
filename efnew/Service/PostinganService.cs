using AutoMapper;
using DTOs.Postingan;
using Models;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services;

public class PostinganService : IPostinganService
{
    private readonly IPostinganRepository _postinganRepository;
    private readonly IMapper _mapper;

    public PostinganService(IPostinganRepository postinganRepository, IMapper mapper)
    {
        _postinganRepository = postinganRepository;
        _mapper = mapper;
    }

    public async Task<PostinganResponseDto> CreatePostinganAsync(CreatePostinganDto dto, string userId, string userName)
    {
        var post = _mapper.Map<Postingan>(dto);
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;

        await _postinganRepository.AddAsync(post);
        await _postinganRepository.SaveChangesAsync();

        var response = _mapper.Map<PostinganResponseDto>(post);
        response.UserName = userName;

        return response;
    }

    public async Task<List<PostinganResponseDto>> GetAllPostinganAsync()
    {
        var posts = await _postinganRepository.GetAllWithUserAsync();
        return _mapper.Map<List<PostinganResponseDto>>(posts);
    }

    public async Task<PostinganResponseDto> UpdatePostinganAsync(Guid id, UpdatePostinganDto dto, string userId)
    {
        var post = await _postinganRepository.GetByIdWithUserAsync(id);

        if (post is null)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _mapper.Map(dto, post);
        post.UpdatedAt = DateTime.UtcNow;

        await _postinganRepository.SaveChangesAsync();

        return _mapper.Map<PostinganResponseDto>(post);
    }

    public async Task DeletePostinganAsync(Guid id, string userId)
    {
        var post = await _postinganRepository.GetByIdAsync(id);

        if (post is null)
            throw new KeyNotFoundException("Postingan tidak ditemukan.");

        if (post.UserId != userId)
            throw new UnauthorizedAccessException("Anda tidak memiliki akses.");

        _postinganRepository.Remove(post);
        await _postinganRepository.SaveChangesAsync();
    }
}
