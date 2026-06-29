using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Repositories;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserRepository _repo;

    public UsersController(UserRepository repo)
    {
        _repo = repo;
    }

    // GET /api/users
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _repo.GetAllAsync();
        return Ok(users);
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"User with id {id} not found" });
        return Ok(user);
    }

    // POST /api/users
    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Name and Email are required" });

        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        try
        {
            var id = await _repo.CreateAsync(user);
            user.Id = id;
            return CreatedAtAction(nameof(GetById), new { id }, user);
        }
        catch (Exception ex)
        {
            return Conflict(new { message = $"Email '{request.Email}' already exists", error = ex.Message });
        }
    }

    // PUT /api/users/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Name and Email are required" });

        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"User with id {id} not found" });

        existing.Name = request.Name;
        existing.Email = request.Email;

        try
        {
            var success = await _repo.UpdateAsync(existing);
            if (!success)
                return NotFound(new { message = $"User with id {id} not found" });
            return Ok(existing);
        }
        catch (Exception ex)
        {
            return Conflict(new { message = $"Email '{request.Email}' already exists", error = ex.Message });
        }
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var success = await _repo.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = $"User with id {id} not found" });
        return NoContent();
    }
}