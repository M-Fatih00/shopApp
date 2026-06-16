using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.User;

public class CreateUserDTO
{
    [Required(ErrorMessage = "Email zorunlu")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Ad Soyad zorunlu")]
    public string AdSoyad { get; set; } = null!;

    [Required(ErrorMessage = "Şifre zorunlu")]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    public string? Role { get; set; }
}