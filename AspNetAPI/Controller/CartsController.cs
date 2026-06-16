using System.Security.Claims;
using AspNetAPI.DTO;
using AspNetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetAPI.Controller;

[ApiController]
[Route("api/carts")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("getcart")]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCart(UserId);
        return Ok(cart);
    }


    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto model)
    {
        await _cartService.AddToCart(UserId, model.UrunId, model.Miktar);
        return Ok(new { message = "Ürün sepete eklendi" });
    }


    [HttpPost("remove")]
    public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDto model)
    {
        await _cartService.RemoveItem(UserId, model.UrunId, model.Miktar);
        return Ok(new { message = "Ürün sepetten çıkarıldı" });
    }
}