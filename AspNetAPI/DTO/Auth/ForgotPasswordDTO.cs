using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class ForgotPasswordDTO
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}