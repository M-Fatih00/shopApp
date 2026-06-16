namespace AspNetAPI.Data;

// Entity
public class Categori
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;

    public List<Product> Uruns { get; set; } = new();  // bir kategorinin birden fazla ürünü olabileceği için list şeklinde yazdık
}