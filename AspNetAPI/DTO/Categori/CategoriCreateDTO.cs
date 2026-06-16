using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class CategoriCreateDTO
{
    [Required]
    [StringLength(30)]
    [Display(Name ="Kategori Adı")]
    public string KategoriAdi { get; set; } = null!;

    [Display(Name ="URL")]
    [Required]
    [StringLength(30)]
    public string Url { get; set; } = null!;
}