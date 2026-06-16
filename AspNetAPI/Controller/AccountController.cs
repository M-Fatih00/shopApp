using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetAPI.Data;
using AspNetAPI.DTO;
using AspNetAPI.DTO.Auth;
using AspNetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace AspNetAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDTO model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var user = new AppUser
        {
            AdSoyad = model.AdSoyad,
            UserName = model.UserName,
            Email = model.Email,
            DateAdded = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            return StatusCode(201);
        }
        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return BadRequest(new { message = "email hatalı" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded)
        {
            return Ok(
                new { token = await GenerateJWT(user) }
            );
        }
        return Unauthorized();
    }


    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        return Ok(new
        {
            userName = user.UserName,
            adSoyad = user.AdSoyad,
            email = user.Email
        });
    }


    [Authorize]
    [HttpPut("edit-profile")]
    public async Task<IActionResult> EditProfile(EditProfileDTO model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        user.UserName = model.UserName;
        user.AdSoyad = model.AdSoyad;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Profil güncellendi" });
    }



    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(AccountChangePasswordDTO model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Giriş yapan kullanıcıyı al
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized(new { message = "Kullanıcı bulunamadı." });

        // Şifre değiştir
        var result = await _userManager.ChangePasswordAsync(
            user,
            model.OldPassword,
            model.Password
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Parola başarıyla değiştirildi." });
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return BadRequest(new { message = "Eposta adresi giriniz" });
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound(new { message = "Bu eposta adresi kayıtlı değildir" });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // Token URL encode edilmeli
        token = Uri.EscapeDataString(token);

        // Frontend URL (MVC / React vs.)
        var resetLink =
            $"http://localhost:5045/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={token}";

        var htmlMessage =
            $"<a href='{resetLink}'>Şifre Yenile</a>";

        await _emailService.SendEmailAsync(
            user.Email!,
            "Parola Sıfırlama",
            htmlMessage
        );

        return Ok(new
        {
            message = "Şifre sıfırlama linki e-posta adresinize gönderildi"
        });
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            return BadRequest(new { message = "Geçersiz istek" });

        var result = await _userManager.ResetPasswordAsync(
            user,
            model.Token,
            model.Password
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Parola başarıyla sıfırlandı." });
    }



    private async Task<string> GenerateJWT(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(
            _configuration.GetSection("AppSettings:Secret").Value ?? ""
        );

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        // rol kısmı
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = "fatihcanibek.com"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}