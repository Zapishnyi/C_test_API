using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Models.DTOs;
using MyApp.Models.Entities;
using MyApp.Repositories;
using Npgsql;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly PostRepository _repo;

    public PostsController(PostRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetAll()
    {
        var posts = await _repo.GetAllAsync();
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetById(Guid id)
    {
        var post = await _repo.GetByIdAsync(id);
        if (post == null)
            return NotFound(new { message = $"Post with id {id} not found" });
        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<Post>> Create([FromBody] CreatePostRequest request)
    {
        var post = new Post
        {
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var createdPost = await _repo.CreateAsync(post);
        return CreatedAtAction(nameof(GetById), new { id = createdPost.Id }, createdPost);
    }
}
