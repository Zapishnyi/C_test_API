namespace MyApp.Models.DTOs;

public class PostDtoRes
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
}
