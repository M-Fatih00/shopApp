using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO.User;

public class CreateUserDTO
{
    public string Email { get; set; } = null!;
    public string AdSoyad { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Role { get; set; }

}