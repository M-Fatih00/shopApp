
namespace AspNetAPI.DTO;

public class CreateProductDTO
{
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Aciklama { get; set; }
    public bool Anasayfa { get; set; }
    public int KategoriId { get; set; }
    public bool Aktif { get; set; }
    public IFormFile Resim { get; set; } = null!;
}