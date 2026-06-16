using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.User;

public class EditUserDTO
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string AdSoyad { get; set; } = null!;

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public string? Role { get; set; }
    public List<string>? AvailableRoles { get; set; }
    public List<string>? SelectedRoles { get; set; }


    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
    public string? ConfirmPassword { get; set; }
}