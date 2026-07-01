using MyApp.Models.DTOs;
using MyApp.Models.Entities;

namespace Helpers;

public class ToDto
{
    public static PostDtoRes Post(Post post)
    {
        return new PostDtoRes
        {
            Id = post.Id,
            Content = post.Content,
            UpdatedAt = post.UpdatedAt,
            CreatedAt = post.CreatedAt,
            UserId = post.UserId,
        };
    }
}
