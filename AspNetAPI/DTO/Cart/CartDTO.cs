namespace AspNetAPI.DTO;

public class CartDTO
{
    public int CartId { get; set; }
    public string CustomerId { get; set; } = null!;
    public List<CartItemDTO> CartItems { get; set; } = new();
    public string Username { get; set; } = null!;
    public double AraToplam { get; set; }
    public decimal Toplam { get; set; }
}


public class CartItemDTO
{
    public int CartItemId { get; set; }

    public int UrunId { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string Resim { get; set; } = null!;

    public int Miktar { get; set; }
}