using Microsoft.AspNetCore.Mvc;
using AspNetUI.ViewModels;
using AspNetUI.DTOs.Product;
using AspNetUI.DTOs.Categori;

namespace AspNetUI.Controllers;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("api");  // IHttpClientFactory kullanma sebebimiz daha güvenli ve performanslı.
    }


    // public async Task<IActionResult> Index()
    // {
    //     var products = _httpClient.GetFromJsonAsync<List<ListProductsDTO>>("products");
    //     var categories = _httpClient.GetFromJsonAsync<List<CategoriListDTO>>("categories");

    //     await Task.WhenAll(products!, categories!);

    //     var model = new HomeIndexViewModel
    //     {
    //         Products = products.Result!,
    //         Categories = categories.Result!
    //     };

    //     return View(model);
    // }

    public async Task<IActionResult> Index()
    {
        // API'den tüm ürünleri ve kategorileri çekiyoruz
        var productsTask = _httpClient.GetFromJsonAsync<List<ListProductsDTO>>("products");
        var categoriesTask = _httpClient.GetFromJsonAsync<List<CategoriListDTO>>("categories");

        await Task.WhenAll(productsTask!, categoriesTask!);

        var products = productsTask.Result ?? new List<ListProductsDTO>();
        var categories = categoriesTask.Result ?? new List<CategoriListDTO>();

        // Ürünleri ID'ye göre tersten sıralayıp (en son eklenen en büyük ID'dir) 
        // sadece ilk 4 tanesini alıyoruz.
        var lastFourProducts = products
            .OrderByDescending(x => x.Id)
            .Take(4)
            .ToList();

        var model = new HomeIndexViewModel
        {
            Products = lastFourProducts,
            Categories = categories
        };

        return View(model);
    }

}