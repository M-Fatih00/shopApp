using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AspNetUI.DTOs.Product;

public class UpdateProductDTO
{
    [Required]
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Aciklama { get; set; }
    public bool Anasayfa { get; set; }
    public int KategoriId { get; set; }
    public bool Aktif { get; set; }

    public string? ResimAdi { get; set; }

    [JsonIgnore]
    public IFormFile? Resim { get; set; }

}