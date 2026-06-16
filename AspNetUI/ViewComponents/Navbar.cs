using Microsoft.AspNetCore.Mvc;
using AspNetUI.DTOs.Categori;

namespace AspNetUI.ViewComponents;

public class Navbar : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public Navbar(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5237/");

        var kategoriler =
            await client.GetFromJsonAsync<List<CategoriListDTO>>(
                "api/categories"
            );

        return View(kategoriler ?? new List<CategoriListDTO>());
    }
}