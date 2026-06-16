using System.ComponentModel.DataAnnotations;

namespace AspNetUI.DTOs.Product;

public class ProductDTO
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public decimal Fiyat { get; set; }
    public string? Resim { get; set; }

}