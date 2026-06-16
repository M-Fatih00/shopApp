namespace AspNetAPI.Data;

public class Product
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Resim { get; set; }
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }
    public int Stok { get; set; }

    public int KategoriId { get; set; }
    
    public Categori Kategori { get; set; } = null!;  // bir ürünün bir kategorisi olduğu için List yapmadık
}