namespace AspNetAPI.DTO;

public class OrderDTO
{
    public int Id { get; set; }
    public DateTime SiparisTarihi { get; set; } = DateTime.Now;
    public string AdSoyad { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Sehir { get; set; } = null!;
    public string AdresSatiri { get; set; } = null!;
    public string? CustomerId { get; set; }
    public string PostaKodu { get; set; } = null!;
    public string Telefon { get; set; } = null!;
    public string SiparisNotu { get; set; } = null!;
    public List<OrderItemDTO> OrderItems { get; set; } = new();
    public decimal AraToplam { get; set; }
    public decimal TeslimatUcreti { get; set; }

}

public class OrderItemDTO
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public string UrunAdi { get; set; } = null!;
    public string UrunResmi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public int Miktar { get; set; }
}
