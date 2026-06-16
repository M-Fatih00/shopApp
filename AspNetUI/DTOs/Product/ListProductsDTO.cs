namespace AspNetUI.DTOs.Product;

public class ListProductsDTO
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Resim { get; set; }
    public string? KategoriAdi { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }
}