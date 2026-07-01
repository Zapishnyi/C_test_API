namespace MyApp.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class UserDtoReq
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
