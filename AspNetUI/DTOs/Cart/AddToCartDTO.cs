namespace AspNetUI.DTOs.Cart;

public class AddToCartDto
{
    public int UrunId { get; set; }
    public int Miktar { get; set; } = 1;
}