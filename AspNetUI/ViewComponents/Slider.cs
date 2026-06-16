using Microsoft.AspNetCore.Mvc;
using AspNetUI.DTOs.Slider;

namespace AspNetUI.ViewComponents;

public class Slider : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public Slider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var apiUrl = "http://localhost:5237/";
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(apiUrl);

        var sliders = await client.GetFromJsonAsync<List<ListSlidersDTO>>("api/slidersapi/active");

        ViewBag.ApiUrl = apiUrl;
        return View(sliders ?? new List<ListSlidersDTO>());
    }
}