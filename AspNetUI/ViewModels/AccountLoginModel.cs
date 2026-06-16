using System.ComponentModel.DataAnnotations;

namespace AspNetUI.ViewModels;

public class AccountLoginModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public bool BeniHatirla { get; set; }
}