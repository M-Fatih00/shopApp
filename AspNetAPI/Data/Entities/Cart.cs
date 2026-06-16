namespace AspNetAPI.Data;

public class Cart
{
    public int CartId { get; set; }
    public string CustomerId { get; set; } = null!;

    public List<CartItem> CartItems { get; set; } = new();

    public void AddItem(Product urun, int miktar)
    {
        var item = CartItems.Where(i => i.UrunId == urun.Id).FirstOrDefault();

        if(item == null)
        {
            CartItems.Add(new CartItem { Urun = urun, Miktar =  miktar});
        }
        else
        {
            item.Miktar += miktar;
        }
    }

    public void DeleteItem(int urunId, int miktar)
    {
        var item = CartItems.Where(i => i.UrunId == urunId).FirstOrDefault();

        if(item != null)
        {
            item.Miktar -= miktar;

            if(item.Miktar == 0)
            {
                CartItems.Remove(item);
            }
        }
    }

    public decimal AraToplam()
    {
        return CartItems.Sum(i => i.Urun.Fiyat * i.Miktar);
    }

    public decimal Toplam()
    {
        return CartItems.Sum(i => i.Urun.Fiyat * i.Miktar)* 1.2m;
    }
}

public class CartItem
{
    public int CartItemId { get; set; }
    public int UrunId { get; set; }
    public Product Urun { get; set; } = null!;
    public int CartId { get; set; }
    public Cart Cart { get; set; } = null!;
    public int Miktar { get; set; }
}