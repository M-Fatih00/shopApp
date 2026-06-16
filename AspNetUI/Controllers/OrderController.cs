using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetUI.DTOs.Cart;
using AspNetUI.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5237/");

        // Token'ı cookie'den alıyoruz
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

        var response = await client.GetAsync("api/orders/GetAllOrders");

        var orders = new List<OrderDTO>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();

            orders = JsonSerializer.Deserialize<List<OrderDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<OrderDTO>();
        }

        return View(orders);
    }


    public async Task<IActionResult> OrderList()
    {
        var client = CreateClient();

        var response = await client.GetAsync("api/orders/GetOrders");
        List<OrderDTO> orders;

        if (!response.IsSuccessStatusCode)
        {
            orders = new List<OrderDTO>();
            ViewBag.Warning = "Siparişler alınamadı, lütfen daha sonra tekrar deneyin.";
        }
        else
        {
            var json = await response.Content.ReadAsStringAsync();
            orders = JsonSerializer.Deserialize<List<OrderDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDTO>();

            if (orders.Count == 0)
                ViewBag.Warning = "Henüz siparişiniz bulunmamaktadır.";
        }

        return View(orders);
    }


    public async Task<IActionResult> Details(int id)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"api/orders/{id}");
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var json = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<OrderDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (order == null) return NotFound();

        return View(order);
    }


    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var client = CreateClient();

        var response = await client.GetAsync("api/carts/getcart");

        CartDTO? cart = null;

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            cart = JsonSerializer.Deserialize<CartDTO>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
        else
        {
            ViewBag.Warning = "Sepetiniz bulunamadı.";
        }

        ViewBag.Cart = cart ?? new CartDTO();

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Checkout(CreateOrderDTO model)
    {
        var client = CreateClient();

        var cartResponse = await client.GetAsync("api/carts/getcart");
        if (cartResponse.IsSuccessStatusCode)
        {
            var cartJson = await cartResponse.Content.ReadAsStringAsync();
            ViewBag.Cart = JsonSerializer.Deserialize<CartDTO>(
                cartJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        if (!ModelState.IsValid)
            return View(model);

        var content = new StringContent(
            JsonSerializer.Serialize(model),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/orders/create", content);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Warning = "Sipariş oluşturulamadı.";
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<OrderDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return RedirectToAction("Completed", new { orderId = order!.Id });
    }


    [Authorize]
    public async Task<IActionResult> Completed(int orderId)
    {
        var client = CreateClient();

        // API'den siparişi çekiyoruz
        var response = await client.GetAsync($"api/orders/{orderId}");
        if (!response.IsSuccessStatusCode)
        {
            TempData["Warning"] = "Sipariş bulunamadı.";
            return RedirectToAction("Index", "Home");
        }

        var json = await response.Content.ReadAsStringAsync();

        var order = JsonSerializer.Deserialize<OrderDTO>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (order == null)
        {
            TempData["Warning"] = "Sipariş bilgileri alınamadı.";
            return RedirectToAction("Index", "Home");
        }

        return View("Completed", order);
    }


}