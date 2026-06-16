using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class EditRoleDTO
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(30)]
    [Display(Name ="Role Adı")]
    public string RoleAdi { get; set; } = null!;
}