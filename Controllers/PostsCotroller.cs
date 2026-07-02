using Microsoft.AspNetCore.Mvc;
using MyApp.Models.DTOs;
using MyApp.Services;

namespace MyApp.Controllers;

[ApiController]
[Route("api/users/{userId}/posts")]
public class PostsController : ControllerBase
{
    private readonly PostService _service;

    public PostsController(PostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<PostDtoRes>>> GetAll(Guid userId)
    {
        var posts = await _service.GetAllAsync(userId);
        return Ok(posts);
    }

    [HttpGet("{postId}")]
    public async Task<ActionResult<PostDtoRes>> GetById(Guid userId, Guid postId)
    {
        var post = await _service.GetByIdAsync(userId, postId);
        if (post == null)
            return NotFound(new { message = $"Post with id {postId} not found" });
        return Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<PostDtoRes>> Create(Guid userId, [FromBody] PostDtoReq request)
    {
        var createdPost = await _service.CreateAsync(userId, request);
        return CreatedAtAction(
            nameof(GetById),
            new { postId = createdPost.Id, userId },
            createdPost
        );
    }

    [HttpPut("{postId}")]
    public async Task<ActionResult> Update(Guid userId, Guid postId, [FromBody] PostDtoReq request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _service.UpdateAsync(userId, postId, request);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{postId}")]
    public async Task<ActionResult> Delete(Guid userId, Guid postId)
    {
        try
        {
            await _service.DeleteAsync(userId, postId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
