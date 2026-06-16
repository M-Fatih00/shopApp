using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.Auth;

public class ForgotPasswordDTO
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}