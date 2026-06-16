using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetUI.DTOs.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RoleController(IHttpClientFactory httpClientFactory)
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

    // GET: /Role
    public async Task<IActionResult> Index()
    {
        var client = CreateClient();
        var response = await client.GetAsync("api/roles");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Roller yüklenemedi.";
            return View(new List<EditRoleDTO>()); // DTO listesi
        }

        var json = await response.Content.ReadAsStringAsync();
        var roles = JsonSerializer.Deserialize<List<EditRoleDTO>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(roles);
    }

    // GET: /Role/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Role/Create
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var client = CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PostAsync("api/Roles", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", error);
        return View(dto);
    }

    // GET: /Role/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Index");

        var client = CreateClient();
        var response = await client.GetAsync($"api/roles/{id}");

        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var json = await response.Content.ReadAsStringAsync();
        var role = JsonSerializer.Deserialize<EditRoleDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (role == null)
            return RedirectToAction("Index");

        return View(role);
    }

    // POST: /Role/Edit/5
    [HttpPost]
    public async Task<IActionResult> Edit(string id, EditRoleDTO dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var client = CreateClient();

        var content = new StringContent(
            JsonSerializer.Serialize(dto),
            Encoding.UTF8,
            "application/json"
        );

        var response = await client.PutAsync($"api/roles/{id}", content);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", error);
        return View(dto);
    }

    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

        var client = CreateClient();
        var response = await client.GetAsync($"api/Roles/{id}");

        if (!response.IsSuccessStatusCode) return RedirectToAction("Index");

        var json = await response.Content.ReadAsStringAsync();
        var role = JsonSerializer.Deserialize<EditRoleDTO>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        ViewBag.Users = new List<string>();

        return View(role);
    }

    // POST: /Role/DeleteConfirm/5
    [HttpPost]
    public async Task<IActionResult> DeleteConfirm(string id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToAction("Index");

        var client = CreateClient();
        var response = await client.DeleteAsync($"api/Roles/{id}");

        if (response.IsSuccessStatusCode)
            TempData["Mesaj"] = "Rol silindi.";
        else
            TempData["Error"] = await response.Content.ReadAsStringAsync();

        return RedirectToAction("Index");
    }
}