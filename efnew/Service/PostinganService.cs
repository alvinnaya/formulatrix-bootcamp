using AutoMapper;
using DTOs.Postingan;
using Common;
using DTOs.Common;
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

    public async Task<Result<PostinganResponseDto>> CreatePostinganAsync(CreatePostinganDto dto, string userId, string userName)
    {
        var post = _mapper.Map<Postingan>(dto);
        post.UserId = userId;
        post.CreatedAt = DateTime.UtcNow;

        await _postinganRepository.AddAsync(post);
        await _postinganRepository.SaveChangesAsync();

        var response = _mapper.Map<PostinganResponseDto>(post);
        response.UserName = userName;

        return Result<PostinganResponseDto>.Ok(response);
    }

    public async Task<Result<List<PostinganResponseDto>>> GetAllPostinganAsync()
    {
        var posts = await _postinganRepository.GetAllWithUserAsync();
        return Result<List<PostinganResponseDto>>.Ok(_mapper.Map<List<PostinganResponseDto>>(posts));
    }

    public async Task<Result<PostinganResponseDto>> UpdatePostinganAsync(Guid id, UpdatePostinganDto dto, string userId)
    {
        var post = await _postinganRepository.GetByIdWithUserAsync(id);

        if (post is null)
            return Result<PostinganResponseDto>.Fail("Postingan tidak ditemukan.", 404);

        if (post.UserId != userId)
            return Result<PostinganResponseDto>.Fail("Anda tidak memiliki akses.", 403);

        _mapper.Map(dto, post);
        post.UpdatedAt = DateTime.UtcNow;

        await _postinganRepository.SaveChangesAsync();

        return Result<PostinganResponseDto>.Ok(_mapper.Map<PostinganResponseDto>(post));
    }

    public async Task<Result<MessageResponseDto>> DeletePostinganAsync(Guid id, string userId)
    {
        var post = await _postinganRepository.GetByIdAsync(id);

        if (post is null)
            return Result<MessageResponseDto>.Fail("Postingan tidak ditemukan.", 404);

        if (post.UserId != userId)
            return Result<MessageResponseDto>.Fail("Anda tidak memiliki akses.", 403);

        _postinganRepository.Remove(post);
        await _postinganRepository.SaveChangesAsync();

        return Result<MessageResponseDto>.Ok(new MessageResponseDto
        {
            Message = "Postingan berhasil dihapus."
        });
    }
}
