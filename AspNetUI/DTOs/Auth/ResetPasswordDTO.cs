using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.Auth;

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


    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor")]
    [Display(Name = "Parola Tekrar")]
    public string ConfirmPassword { get; set; } = null!;
}