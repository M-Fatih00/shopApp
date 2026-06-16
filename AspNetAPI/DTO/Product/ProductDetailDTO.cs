namespace AspNetAPI.DTO;

public class ProductDetailDTO
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Aciklama { get; set; }
    public bool Anasayfa { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string? Resim { get; set; }

}