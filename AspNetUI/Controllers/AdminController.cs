using System.Net.Http.Headers;
using System.Text.Json;
using AspNetUI.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AdminController(IHttpClientFactory httpClientFactory)
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
        var response = await client.GetAsync("api/orders/GetRecentOrders");

        List<OrderDTO> orders = new List<OrderDTO>();
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            orders = JsonSerializer.Deserialize<List<OrderDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OrderDTO>();
        }

        return View(orders);
    }

}