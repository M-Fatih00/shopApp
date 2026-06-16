using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetUI.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;


public class CartController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CartController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5237/");

        var token = Request.Cookies["token"];

        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<IActionResult> Index()
    {
        var client = CreateClient();

        var response = await client.GetAsync("api/carts/getcart");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var json = await response.Content.ReadAsStringAsync();
        var cart = JsonSerializer.Deserialize<CartDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int urunId, int miktar = 1)
    {
        var client = CreateClient();

        var dto = new AddToCartDto
        {
            UrunId = urunId,
            Miktar = miktar
        };

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json"
        );

        await client.PostAsync("api/carts/add", content);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveItem(int urunId, int miktar)
    {
        var client = CreateClient();

        var dto = new RemoveFromCartDto
        {
            UrunId = urunId,
            Miktar = miktar
        };

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json"
        );

        await client.PostAsync("api/carts/remove", content);

        return RedirectToAction("Index");
    }

}