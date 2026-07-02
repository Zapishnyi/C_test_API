using Helpers;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models.DTOs;
using MyApp.Models.Entities;
using MyApp.Repositories;

namespace MyApp.Controllers;

[ApiController]
[Route("api/users/{userId}/posts")]
public class PostsController : ControllerBase
{
    private readonly PostRepository _repo;

    public PostsController(PostRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<PostDtoRes>>> GetAll(Guid userId)
    {
        var posts = await _repo.GetAllAsync(userId);
        var postDtos = posts.Select(ToDto.Post).ToList();
        return Ok(postDtos);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDtoRes>> GetById(Guid userId, Guid postId)
    {
        var post = await _repo.GetByIdAsync(userId, postId);
        if (post == null)
            return NotFound(new { message = $"Post with id {postId} not found" });
        return Ok(ToDto.Post(post));
    }

    [HttpPost]
    public async Task<ActionResult<PostDtoRes>> Create(Guid userId, [FromBody] PostDtoReq request)
    {
        var post = new Post
        {
            Content = request.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var createdPost = await _repo.CreateAsync(post);
        return CreatedAtAction(
            nameof(GetById),
            new { postId = createdPost.Id, userId },
            ToDto.Post(createdPost)
        );
    }

    [HttpPut("{postId}")]
    public async Task<ActionResult> Update(Guid userId, Guid postId, [FromBody] PostDtoReq request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _repo.GetByIdAsync(userId, postId);
        if (existing == null)
            return NotFound(new { message = $"Post with id {postId} not found" });

        existing.Content = request.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> Delete(Guid userId, Guid postId)
    {
        var existing = await _repo.GetByIdAsync(userId, postId);
        if (existing == null)
            return NotFound(new { message = $"Post with id {postId} not found" });

        await _repo.DeleteAsync(postId);
        return NoContent();
    }
}
