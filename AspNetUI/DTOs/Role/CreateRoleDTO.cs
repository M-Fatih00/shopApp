using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.Role;

public class CreateRoleDTO
{
    [Required]
    [StringLength(30)]
    [Display(Name ="Role Adı")]
    public string RoleAdi { get; set; } = null!;
}