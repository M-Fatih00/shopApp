using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

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


    // Kullanıcının birden fazla rolü olabilir
    public List<string> SelectedRoles { get; set; } = new List<string>();

    public string? Password { get; set; }
}