namespace AspNetUI.DTOs.Cart;

public class RemoveFromCartDto
{
    public int UrunId { get; set; }
    public int Miktar { get; set; } = 1;
}