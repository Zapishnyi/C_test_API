using Helpers;
using MyApp.Models.DTOs;
using MyApp.Models.Entities;
using MyApp.Repositories;

namespace MyApp.Services;

public class PostService
{
    private readonly PostRepository _repo;

    public PostService(PostRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<PostDtoRes>> GetAllAsync(Guid userId)
    {
        var posts = await _repo.GetAllAsync(userId);
        return posts.Select(ToDto.Post).ToList();
    }

    public async Task<PostDtoRes?> GetByIdAsync(Guid userId, Guid postId)
    {
        var post = await _repo.GetByIdAsync(userId, postId);
        return post is null ? null : ToDto.Post(post);
    }

    public async Task<PostDtoRes> CreateAsync(Guid userId, PostDtoReq request)
    {
        var post = new Post
        {
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var created = await _repo.CreateAsync(post);
        return ToDto.Post(created);
    }

    public async Task UpdateAsync(Guid userId, Guid postId, PostDtoReq request)
    {
        var existing = await _repo.GetByIdAsync(userId, postId);
        if (existing == null)
            throw new KeyNotFoundException($"Post with id {postId} not found");

        existing.Content = request.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid userId, Guid postId)
    {
        var existing = await _repo.GetByIdAsync(userId, postId);
        if (existing == null)
            throw new KeyNotFoundException($"Post with id {postId} not found");

        await _repo.DeleteAsync(postId);
    }
}
