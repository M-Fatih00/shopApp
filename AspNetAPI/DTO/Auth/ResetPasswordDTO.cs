using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class ResetPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;
}