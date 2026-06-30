using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models.Entities;

namespace MyApp.Repositories;

public class UserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task CreateTableIfNotExistsAsync()
    {
        await _db.Database.EnsureCreatedAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _db.Users.OrderBy(u => u.Id).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existing = await _db.Users.FindAsync(user.Id);
        if (existing == null)
            return false;

        existing.Name = user.Name;
        existing.Email = user.Email;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
            return false;

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return true;
    }
}
