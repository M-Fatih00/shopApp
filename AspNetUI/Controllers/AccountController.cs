using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetUI.DTOs.Auth;
using AspNetUI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetUI.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
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

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(RegisterDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "api/account/register",
            model
        );

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Login");

        ModelState.AddModelError("", "Kayıt başarısız");
        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountLoginModel model, string? returnUrl)
    {
        if (!ModelState.IsValid)
            return View(model);

        // View Model → API DTO dönüşümü
        var dto = new LoginDTO
        {
            Email = model.Email,
            Password = model.Password
        };

        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "api/account/login",
            dto
        );

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Email veya parola hatalı");
            return View(model);
        }

        var result =
            await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

        var token = result!.Token;



        var expireDays = model.BeniHatirla ? 7 : 1;

        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.Now.AddDays(expireDays)
        });

        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }



    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return RedirectToAction("Login", "Account");
    }


    [Authorize]
    public async Task<ActionResult> EditUser()
    {
        var client = CreateClient();

        var response = await client.GetAsync("api/account/profile");
        if (!response.IsSuccessStatusCode)
            return RedirectToAction("Login", "Account");

        var user = await response.Content
            .ReadFromJsonAsync<AccountEditUserModel>();

        return View(user);
    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult> EditUser(AccountEditUserModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();

        var response = await client.PutAsJsonAsync(
            "api/account/edit-profile",
            model
        );

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Bilgileriniz güncellendi.";
            // return View(model);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Güncelleme başarısız");
        return View(model);
    }


    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(AccountChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "api/account/change-password",
            model
        );

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Parolanız güncellendi";
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Parola değiştirilemedi");
        return View(model);
    }


    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "api/account/forgot-password",
            model
        );

        TempData["Mesaj"] =
            response.IsSuccessStatusCode
            ? "Şifre sıfırlama linki mail adresinize gönderildi"
            : "Bir hata oluştu";

        return RedirectToAction("Login");
    }


    public IActionResult ResetPassword(string email, string token)
    {
        return View(new ResetPasswordDTO
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = CreateClient();
        var response = await client.PostAsJsonAsync(
            "api/account/reset-password",
            model
        );

        if (response.IsSuccessStatusCode)
        {
            TempData["Mesaj"] = "Şifreniz güncellendi";
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "Şifre sıfırlanamadı");
        return View(model);
    }
}