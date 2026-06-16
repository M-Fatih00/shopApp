using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetUI.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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

    
    public async Task<IActionResult> Index(string? role)
    {
        var client = CreateClient();
        var response = await client.GetAsync("api/users");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var json = await response.Content.ReadAsStringAsync();
        var users = JsonSerializer.Deserialize<List<UserDTO>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<UserDTO>();

        // Roller dropdown için
        ViewBag.Roller = new SelectList(new List<string> { "Admin", "Customer" }, role);

        // Filtreleme: seçilen role’a sahip kullanıcıları al
        if (!string.IsNullOrEmpty(role))
        {
            users = users.Where(u => u.Roles != null && u.Roles.Contains(role)).ToList();
        }

        return View(users);
    }

    public ActionResult Create()
    {
        // Roller ve default seçili "Customer" rolü
        ViewBag.Roles = new SelectList(new List<string> { "Admin", "Customer" }, "Customer");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(model),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/users/create", content);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Kullanıcı oluşturulamadı");
            return View(model);
        }

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Edit(string id)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"api/users/{id}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var roles = new List<string> { "Admin", "Customer" };

        return View(new EditUserDTO
        {
            Id = user!.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            AdSoyad = user.AdSoyad,
            SelectedRoles = user.Roles,
            AvailableRoles = roles
        });
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditUserDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Şifre girilmediyse gönderme
        if (string.IsNullOrEmpty(model.Password))
        {
            model.Password = null;
            model.ConfirmPassword = null;
        }

        var client = CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(model),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PutAsync("api/users/edit", content);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Kullanıcı güncellenemedi");
            return View(model);
        }

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(string id)
    {
        var client = CreateClient();

        var response = await client.GetAsync($"api/users/{id}");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var json = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(user);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(string id)
    {
        var client = CreateClient();

        var response = await client.DeleteAsync($"api/users/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Kullanıcı silindi";
        }

        return RedirectToAction("Index");
    }

}