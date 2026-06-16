namespace AspNetUI.DTOs.Product;

public class ProductDetailDTO
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Aciklama { get; set; }
    public bool Anasayfa { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public int KategoriId { get; set; }
    public string? ResimAdi { get; set; }

}