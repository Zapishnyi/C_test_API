namespace MyApp.Models.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
