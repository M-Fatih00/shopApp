using System.ComponentModel.DataAnnotations;

namespace AspNetAPI.DTO;

public class ProductDTO
{
    [Display(Name = "Ürün adı")]
    [Required(ErrorMessage = "{0} girmelisiniz!")]
    [StringLength(50, ErrorMessage = "{0} için {2}-{1} karakter aralığında değer girmelisiniz.", MinimumLength = 10)]
    public string UrunAdi { get; set; } = null!;
    

    [Display(Name = "Ürün fiyat")]
    [Required(ErrorMessage = "{0} zorunlu.")]
    [Range(0, 1000000, ErrorMessage = "{0} için girdiğiniz değer {1} ile {2} arasında olmalıdır.")]
    public decimal Fiyat { get; set; }


    [Display(Name = "Ürün Resmi")]
    public IFormFile? Resim { get; set; }


    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }


    [Display(Name = "Anasayfa")]
    public bool Anasayfa { get; set; }


    [Display(Name = "Kategori")]
    [Required(ErrorMessage = "{0} zorunlu.")]
    public int KategoriId { get; set; }
}