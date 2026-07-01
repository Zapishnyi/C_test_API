using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Models.DTOs;
using MyApp.Models.Entities;
using MyApp.Repositories;
using Npgsql;

namespace MyApp.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _repo;

    public UsersController(UserRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _repo.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> GetById(Guid userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = $"User with id {userId} not found" });
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] UserDtoReq request)
    {
        var user = new User { Name = request.Name, Email = request.Email };

        try
        {
            var createdUser = await _repo.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { userId = createdUser.Id }, createdUser);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            return Conflict(new { message = $"Email '{request.Email}' already exists" });
        }
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> Update(Guid userId, [FromBody] UserDtoReq request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _repo.GetByIdAsync(userId);
        if (existing == null)
            return NotFound(new { message = $"User with id {userId} not found" });

        existing.Name = request.Name;
        existing.Email = request.Email;

        try
        {
            var success = await _repo.UpdateAsync(existing);
            if (!success)
                return NotFound(new { message = $"User with id {userId} not found" });
            return Ok(existing);
        }
        catch (Exception ex)
        {
            return Conflict(
                new { message = $"Email '{request.Email}' already exists", error = ex.Message }
            );
        }
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid userId)
    {
        var success = await _repo.DeleteAsync(userId);
        if (!success)
            return NotFound(new { message = $"User with id {userId} not found" });
        return NoContent();
    }
}
