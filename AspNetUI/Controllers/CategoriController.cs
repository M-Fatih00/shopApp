using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetUI.DTOs.Categori;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriController : Controller
{
    private readonly HttpClient _httpClient;

    public CategoriController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("api");
    }

    private HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5237/");

        var token = Request.Cookies["token"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }



    [AllowAnonymous]
    public async Task<ActionResult> Index()
    {
        var kategoriler = await _httpClient.GetFromJsonAsync<List<CategoriListDTO>>("categories");
        return View(kategoriler);
    }

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(CategoriCreateDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var token = Request.Cookies["token"];

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await _httpClient.PostAsJsonAsync("categories", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ModelState.AddModelError("", "Kategori eklenemedi");
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"api/categories/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }

        var category = await response.Content.ReadFromJsonAsync<CategoriEditDTO>();
        return View(category);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id, CategoriEditDTO model)
    {
        if (id != model.Id)
            return RedirectToAction("Index");

        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient(); 

        var response = await client.PutAsJsonAsync($"api/categories/{id}", model);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ModelState.AddModelError("", "Güncelleme başarısız");
        return View(model);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateClient();
        var category = await client.GetFromJsonAsync<CategoriListDTO>($"api/categories/{id}");

        if (category == null)
            return RedirectToAction("Index");

        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"api/categories/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Kategori silinemedi";
            return RedirectToAction("Delete", new { id });
        }

        TempData["Mesaj"] = "Kategori silindi";
        return RedirectToAction("Index");
    }


}

