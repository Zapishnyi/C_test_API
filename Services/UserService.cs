using Microsoft.EntityFrameworkCore;
using MyApp.Models.DTOs;
using MyApp.Models.Entities;
using MyApp.Repositories;
using Npgsql;

namespace MyApp.Services;

public class UserService
{
    private readonly UserRepository _repo;

    public UserService(UserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _repo.GetByIdAsync(id);
    }

    public async Task<User> CreateAsync(UserDtoReq request)
    {
        var user = new User { Name = request.Name, Email = request.Email };

        try
        {
            return await _repo.CreateAsync(user);
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            throw new InvalidOperationException($"Email '{request.Email}' already exists", ex);
        }
    }

    public async Task<User> UpdateAsync(Guid id, UserDtoReq request)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"User with id {id} not found");

        existing.Name = request.Name;
        existing.Email = request.Email;

        try
        {
            var success = await _repo.UpdateAsync(existing);
            if (!success)
                throw new KeyNotFoundException($"User with id {id} not found");
            return existing;
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            throw new InvalidOperationException($"Email '{request.Email}' already exists", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repo.DeleteAsync(id);
    }
}
