using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetUI.DTOs.Categori;
using AspNetUI.DTOs.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("api");

        var token = Request.Cookies["token"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<IActionResult> Index(int? kategori)
    {
        ViewBag.ApiUrl = "http://localhost:5237/";

        var client = CreateClient();

        var url = kategori == null
            ? "products/admin"
            : $"products/admin?kategoriId={kategori}";

        var products = await client.GetFromJsonAsync<List<ListProductsDTO>>(url);
        var categories = await client.GetFromJsonAsync<List<CategoriListDTO>>("categories");

        ViewBag.Kategoriler = new SelectList(categories, "Id", "KategoriAdi", kategori);

        return View(products);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var client = CreateClient();
        ViewBag.ApiUrl = "http://localhost:5237/";

        // 1. Ana Ürünü Getir
        var product = await client.GetFromJsonAsync<ProductDetailDTO>($"products/{id}");
        if (product == null) return RedirectToAction("Index", "Home");

        // 2. Benzer Ürünleri Yeni Endpoint'ten Getir
        var benzerUrunler = await client.GetFromJsonAsync<List<ListProductsDTO>>($"products/{id}/related");

        ViewData["BenzerUrunler"] = benzerUrunler ?? new List<ListProductsDTO>();

        return View(product);
    }


    [AllowAnonymous]
    [Route("urunler/{url?}")]
    public async Task<IActionResult> List(string? url, string? q)
    {
        var client = CreateClient();

        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(url)) queryParams.Add($"url={url}");
        if (!string.IsNullOrEmpty(q)) queryParams.Add($"q={q}");

        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

        var response = await client.GetAsync("products" + queryString);

        if (!response.IsSuccessStatusCode)
            return View(new List<ListProductsDTO>());

        var jsonData = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ListProductsDTO>>(jsonData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        ViewData["q"] = q;
        ViewBag.ApiUrl = "http://localhost:5237/";

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var client = CreateClient();
        var categories = await client.GetFromJsonAsync<List<CategoriListDTO>>("categories");
        ViewBag.Kategoriler = new SelectList(categories, "Id", "KategoriAdi");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDTO model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = CreateClient();
        var formData = new MultipartFormDataContent();

        formData.Add(new StringContent(model.UrunAdi), "UrunAdi");
        formData.Add(new StringContent(model.Fiyat.ToString(CultureInfo.InvariantCulture)), "Fiyat");
        formData.Add(new StringContent(model.Aciklama ?? ""), "Aciklama");
        formData.Add(new StringContent(model.Aktif.ToString().ToLower()), "Aktif");
        formData.Add(new StringContent(model.Anasayfa.ToString().ToLower()), "Anasayfa");
        formData.Add(new StringContent(model.KategoriId.ToString()), "KategoriId");

        if (model.Resim != null)
        {
            var fileContent = new StreamContent(model.Resim.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.Resim.ContentType);
            formData.Add(fileContent, "Resim", model.Resim.FileName);
        }

        var response = await client.PostAsync("products", formData);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ModelState.AddModelError("", "Ürün eklenemedi.");
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var client = CreateClient();
        var product = await client.GetFromJsonAsync<UpdateProductDTO>($"products/{id}");
        var categories = await client.GetFromJsonAsync<List<CategoriListDTO>>("categories");

        ViewBag.ApiUrl = "http://localhost:5237/";

        ViewBag.Kategoriler = new SelectList(categories, "Id", "KategoriAdi", product!.KategoriId);

        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateProductDTO dto)
    {
        if (id != dto.Id) return RedirectToAction("Index");
        if (!ModelState.IsValid) return View(dto);

        var client = CreateClient();
        var formData = new MultipartFormDataContent();

        // Form verilerini paketleme
        formData.Add(new StringContent(dto.Id.ToString()), "Id");
        formData.Add(new StringContent(dto.UrunAdi), "UrunAdi");
        formData.Add(new StringContent(dto.Fiyat.ToString(CultureInfo.InvariantCulture)), "Fiyat");
        formData.Add(new StringContent(dto.Aciklama ?? ""), "Aciklama");
        formData.Add(new StringContent(dto.Aktif.ToString().ToLower()), "Aktif");
        formData.Add(new StringContent(dto.Anasayfa.ToString().ToLower()), "Anasayfa");
        formData.Add(new StringContent(dto.KategoriId.ToString()), "KategoriId");
        formData.Add(new StringContent(dto.ResimAdi ?? ""), "ResimAdi");

        if (dto.Resim != null)
        {
            var fileContent = new StreamContent(dto.Resim.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.Resim.ContentType);
            formData.Add(fileContent, "Resim", dto.Resim.FileName);
        }

        // PutAsJsonAsync kullanmıyoruz çünkü API [FromForm] bekliyor
        var response = await client.PutAsync($"products/{id}", formData);

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = $"{dto.UrunAdi} ürünü güncellendi.";
            return RedirectToAction("Index");
        }

        var errorMsg = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Güncelleme başarısız: {errorMsg}");
        return View(dto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateClient();
        var product = await client.GetFromJsonAsync<UpdateProductDTO>($"products/{id}");

        if (product == null) return RedirectToAction("Index");
        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"products/{id}");

        if (response.IsSuccessStatusCode)
            TempData["Mesaj"] = "Ürün silindi.";
        else
            TempData["Mesaj"] = "Silme işlemi sırasında bir hata oluştu.";

        return RedirectToAction("Index");
    }
}