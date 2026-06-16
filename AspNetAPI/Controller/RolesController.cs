using AspNetAPI.Data;
using AspNetAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetAPI.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public RolesController(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }


    [HttpGet]
    public IActionResult GetRoles()
    {
        var roles = _roleManager.Roles.Select(r => new
        {
            Id = r.Id,
            RoleAdi = r.Name
        }).ToList();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleWithUsers(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound("Rol bulunamadı");

        return Ok(new
        {
            Id = role.Id,
            RoleAdi = role.Name
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var role = new AppRole
        {
            Name = dto.RoleAdi
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Rol oluşturuldu" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, EditRoleDTO dto)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
            return NotFound();

        role.Name = dto.RoleAdi.Trim();

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Rol güncellendi" });

    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound("Rol bulunamadı");

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            // Hatanın nedenini (mesela 'Role is not empty') UI'a gönder
            return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Ok(new { message = "Rol silindi" });
    }

}