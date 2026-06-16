using AspNetAPI.Data;
using AspNetAPI.DTO;
using AspNetAPI.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }


    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();

        // DTO ile hassas verileri filtreleyelim
        var usersDto = new List<UserDTO>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            usersDto.Add(new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName!,
                AdSoyad = user.AdSoyad,
                Email = user.Email!,
                DateAdded = user.DateAdded,
                Roles = roles.ToList()
            });
        }

        return Ok(usersDto);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        var roles = await _userManager.GetRolesAsync(user!);

        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı" });
        }

        // DTO ile gereksiz bilgileri gizle

        var userDto = new UserDTO
        {
            Id = user.Id,
            AdSoyad = user.AdSoyad,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList(),
        };

        return Ok(userDto);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserDTO model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new AppUser
        {
            UserName = model.Email,
            Email = model.Email,
            AdSoyad = model.AdSoyad,
            DateAdded = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Rol ekleme
        if (!string.IsNullOrEmpty(model.Role))
        {
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new AppRole { Name = model.Role });
            }

            await _userManager.AddToRoleAsync(user, model.Role);
        }

        return Ok(new { message = "Kullanıcı oluşturuldu" });
    }


    [HttpPut("edit")]
    public async Task<IActionResult> EditUser(EditUserDTO model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Kullanıcıyı ID ile bul
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        // Bilgilerini güncelle
        user.AdSoyad = model.AdSoyad;
        user.UserName = model.UserName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!string.IsNullOrEmpty(model.Password))
        {
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, model.Password);
        }

        // Rol değişimi 
        if (model.SelectedRoles != null && model.SelectedRoles.Any())
        {
            foreach (var roleName in model.SelectedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                    await _roleManager.CreateAsync(new AppRole { Name = roleName });

                await _userManager.AddToRoleAsync(user, roleName);
            }
        }

        return Ok(new { message = "Kullanıcı güncellendi." });
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound(new { message = "Kullanıcı bulunamadı." });

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Kullanıcı başarıyla silindi." });
    }

}
