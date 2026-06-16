using AspNetAPI.Data;
using AspNetAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Services;

public interface ICartService
{
    Task<CartDTO> GetCart(string userId);
    Task AddToCart(string userId, int urunId, int miktar = 1);
    Task RemoveItem(string userId, int urunId, int miktar = 1);
}


public class CartService : ICartService
{
    private readonly DataContext _context;

    public CartService(DataContext context)
    {
        _context = context;
    }

    public async Task<CartDTO> GetCart(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Urun)
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null)
            return new CartDTO
            {
                CustomerId = userId,
                CartItems = new List<CartItemDTO>()
            };

        return new CartDTO
        {
            CartId = cart.CartId,
            CustomerId = cart.CustomerId,
            CartItems = cart.CartItems.Select(ci => new CartItemDTO
            {
                CartItemId = ci.CartItemId,
                UrunId = ci.UrunId,
                UrunAdi = ci.Urun.UrunAdi,
                Fiyat = ci.Urun.Fiyat,
                Resim = ci.Urun.Resim!,
                Miktar = ci.Miktar
            }).ToList()
        };
    }


    public async Task AddToCart(string userId, int urunId, int miktar = 1)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null)
        {
            cart = new Cart { CustomerId = userId };
            _context.Carts.Add(cart);
        }

        var cartItem = cart.CartItems
            .FirstOrDefault(i => i.UrunId == urunId);

        if (cartItem == null)
        {
            cart.CartItems.Add(new CartItem
            {
                UrunId = urunId,
                Miktar = miktar
            });
        }
        else
        {
            cartItem.Miktar += miktar;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveItem(string userId, int urunId, int miktar = 1)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == userId);

        if (cart == null) return;

        var cartItem = cart.CartItems
            .FirstOrDefault(ci => ci.UrunId == urunId);

        if (cartItem == null) return;

        cartItem.Miktar -= miktar;

        if (cartItem.Miktar <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }

        await _context.SaveChangesAsync();
    }
}