using System.Data.SqlTypes;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AspNetUI.DTOs.Slider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class SliderController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SliderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5237/"); // API URL

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
        ViewBag.ApiUrl = "http://localhost:5237/";
        var client = CreateClient();

        var sliders = await client.GetFromJsonAsync<List<ListSlidersDTO>>("api/slidersapi/all");
        return View(sliders);
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateSliderDTO model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = CreateClient();

        var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.Baslik ?? ""), "Baslik");
        content.Add(new StringContent(model.Index.ToString()), "Index");
        content.Add(new StringContent(model.Aktif.ToString()), "Aktif");

        if (model.Resim != null)
        {
            var fileStream = model.Resim.OpenReadStream();
            content.Add(new StreamContent(fileStream), "Resim", model.Resim.FileName);
        }

        var response = await client.PostAsync("api/slidersapi", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Slider eklendi";
            return RedirectToAction("Index");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Slider eklenirken hata oluştu: {errorContent}");
        return View(model);
    }


    public async Task<IActionResult> Edit(int id)
    {
        var client = CreateClient();

        var sliderData = await client.GetFromJsonAsync<ListSlidersDTO>($"api/slidersapi/{id}");

        if (sliderData == null) return RedirectToAction("Index");

        var model = new EditSliderDTO
        {
            Id = sliderData.Id,
            Baslik = sliderData.Baslik,
            Index = sliderData.Index,
            Aktif = sliderData.Aktif,
            ResimAdi = sliderData.Resim
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] EditSliderDTO model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var client = CreateClient();

        var content = new MultipartFormDataContent();

        content.Add(new StringContent(model.Id.ToString()), "Id");
        content.Add(new StringContent(model.Baslik ?? ""), "Baslik");
        content.Add(new StringContent(model.Aciklama ?? ""), "Aciklama");
        content.Add(new StringContent(model.Index.ToString()), "Index");
        content.Add(new StringContent(model.Aktif.ToString().ToLower()), "Aktif");
        content.Add(new StringContent(model.ResimAdi ?? ""), "ResimAdi");

        if (model.Resim != null)
        {
            var fileStream = model.Resim.OpenReadStream();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.Resim.ContentType);
            content.Add(fileContent, "Resim", model.Resim.FileName);
        }

        var response = await client.PutAsync($"api/slidersapi/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Slider başarıyla güncellendi";
            return RedirectToAction("Index");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Hata: {errorContent}");
        return View(model);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var client = CreateClient();
        var slider = await client.GetFromJsonAsync<ListSlidersDTO>($"api/slidersapi/{id}");
        if (slider == null) return RedirectToAction("Index");

        return View(slider);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"api/slidersapi/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Slider silindi";
        }

        return RedirectToAction("Index");
    }

}