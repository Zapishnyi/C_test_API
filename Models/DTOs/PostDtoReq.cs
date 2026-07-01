namespace MyApp.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class PostDtoReq
{
    [Required]
    [MinLength(1)]
    [MaxLength(500)]
    public string Content { get; set; } = null!;
}
