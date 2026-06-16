using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class UserDTO
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string AdSoyad { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> Roles { get; set; } = new();  // Sadece admin göreceği için DTO da göstermek güvenlik açısından sorun oluşturmaz.
    public DateTime DateAdded { get; set; }
}
