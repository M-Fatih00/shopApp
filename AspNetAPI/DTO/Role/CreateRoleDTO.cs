using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class CreateRoleDTO
{
    [Required]
    [StringLength(30)]
    [Display(Name ="Role Adı")]
    public string RoleAdi { get; set; } = null!;
}