using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class CategoriDetailDTO
{
    public int Id { get; set; }
    
    [Display(Name ="Kategori Adı")]
    [Required]
    [StringLength(30)]
    public string KategoriAdi { get; set; } = null!;

    [Display(Name ="URL")]
    [Required]
    [StringLength(30)]
    public string Url { get; set; } = null!;

}