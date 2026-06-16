using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO.Auth;

public class EditProfileDTO
{
    [Required]
    public string AdSoyad { get; set; } = null!;


    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    
    [Required]
    public string UserName { get; set; } = null!;

}