using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models.Entities;

namespace MyApp.Repositories;

public class PostRepository
{
    private readonly AppDbContext _db;

    public PostRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Post> CreateAsync(Post post)
    {
        _db.Posts.Add(post);
        await _db.SaveChangesAsync();
        return post;
    }

    public async Task<List<Post>> GetAllAsync(Guid userId)
    {
        return await _db.Posts.Where(p => p.UserId == userId).OrderBy(p => p.Id).ToListAsync();
    }

    public async Task<Post?> GetByIdAsync(Guid userId, Guid postId)
    {
        return await _db
            .Posts.Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
    }

    public async Task<bool> UpdateAsync(Post post)
    {
        var existing = await _db.Posts.FindAsync(post.Id);
        if (existing == null)
            return false;

        existing.Content = post.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var post = await _db.Posts.FindAsync(id);
        if (post == null)
            return false;

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return true;
    }
}
