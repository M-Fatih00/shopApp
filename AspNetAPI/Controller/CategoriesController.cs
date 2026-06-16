using AspNetAPI.Data;
using AspNetAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly DataContext _context;
    public CategoriesController(DataContext context)
    {
        _context = context;
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetKategories()
    {
        var kategoriler = await _context.Categories
            .Select(c => new CategoriListDTO
            {
                Id = c.Id,
                KategoriAdi = c.KategoriAdi,
                Url = c.Url,

                UrunSayisi = c.Uruns.Count()
            })
            .ToListAsync();

        return Ok(kategoriler);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> GetKategori(int? id)
    {
        if (id == null)
            return NotFound();

        var kategori = await _context.Categories.Where(i => i.Id == id).Select(k => CategoriToDTO(k)).FirstOrDefaultAsync();

        if (kategori == null)
            return NotFound();

        return Ok(kategori);
    }


    [HttpPost]
    public async Task<ActionResult> CreateKategori(CategoriCreateDTO dto)
    {
        var kategori = new Categori
        {
            KategoriAdi = dto.KategoriAdi,
            Url = dto.Url
        };

        _context.Categories.Add(kategori);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetKategori), new { id = kategori.Id }, kategori);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> EditKategori(int id, CategoriEditDTO dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var kat = await _context.Categories.FirstOrDefaultAsync(i => i.Id == id);
        if (kat == null)
            return NotFound();

        kat.KategoriAdi = dto.KategoriAdi;
        kat.Url = dto.Url;

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKategori(int id)
    {
        var kategori = await _context.Categories.FirstOrDefaultAsync(i => i.Id == id);

        if (kategori == null)
            return NotFound();

        _context.Categories.Remove(kategori);
        await _context.SaveChangesAsync();

        return NoContent();
    }



    private static CategoriListDTO CategoriToDTO(Categori c)
    {
        var dto = new CategoriListDTO();

        if (c != null)
        {
            dto.Id = c.Id;
            dto.KategoriAdi = c.KategoriAdi;
            dto.Url = c.Url;
        }

        return dto;
    }
}
