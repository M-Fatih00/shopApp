using Microsoft.AspNetCore.Identity;

namespace AspNetAPI.Data;

public class AppUser : IdentityUser<int>
{
    public string AdSoyad { get; set; } = null!;
    public DateTime DateAdded { get; set; }
}