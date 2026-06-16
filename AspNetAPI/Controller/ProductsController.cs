using System.Linq.Expressions;
using AspNetAPI.Data;
using AspNetAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Controller;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;
    public ProductsController(DataContext context)
    {
        _context = context;
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetProducts(
    [FromQuery] string? url,
    [FromQuery] string? q,
    [FromQuery] int? kategoriId)
    {
        var query = _context.Products
            .Where(i => i.Aktif)
            .AsQueryable();

        if (kategoriId != null)
        {
            query = query.Where(i => i.KategoriId == kategoriId);
        }

        if (!string.IsNullOrEmpty(url))
        {
            query = query.Where(i => i.Kategori.Url == url);
        }

        if (!string.IsNullOrEmpty(q))
        {
            query = query.Where(i =>
                i.UrunAdi.ToLower().Contains(q.ToLower()));
        }

        var products = await query.Select(p => new ListProductsDTO
        {
            Id = p.Id,
            UrunAdi = p.UrunAdi,
            Fiyat = p.Fiyat,
            Resim = p.Resim,
            KategoriAdi = p.Kategori.KategoriAdi,
            Aktif = p.Aktif,
            Anasayfa = p.Anasayfa
        }).ToListAsync();

        return Ok(products);
    }

    [HttpGet("admin")]
    public async Task<ActionResult> GetProductsAdmin(int? kategoriId)
    {
        var query = _context.Products.AsQueryable();

        // Sadece kategori seçildiyse filtrele, yoksa hepsini getir
        if (kategoriId != null)
        {
            query = query.Where(i => i.KategoriId == kategoriId);
        }

        var products = await query.Select(p => new ListProductsDTO
        {
            Id = p.Id,
            UrunAdi = p.UrunAdi,
            Fiyat = p.Fiyat,
            Resim = p.Resim,
            KategoriAdi = p.Kategori.KategoriAdi,
            Aktif = p.Aktif,
            Anasayfa = p.Anasayfa
        }).ToListAsync();

        return Ok(products);
    }


    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetProduct(int? id)
    {
        var product = await _context.Products.Where(i => i.Id == id).Select(p => new UpdateProductDTO
        {
            Id = p.Id,
            UrunAdi = p.UrunAdi,
            Fiyat = p.Fiyat,
            Aciklama = p.Aciklama,
            Anasayfa = p.Anasayfa,
            Aktif = p.Aktif,
            KategoriId = p.KategoriId,
            ResimAdi = p.Resim
        }).FirstOrDefaultAsync();

        if (product == null) return NotFound();
        return Ok(product);
    }


    [HttpGet("{id}/related")]
    [AllowAnonymous]
    public async Task<ActionResult> GetRelatedProducts(int id)
    {
        var mainProduct = await _context.Products.FindAsync(id);
        if (mainProduct == null) return NotFound();

        // Aynı kategorideki, aktif olan ve kendisi olmayan ilk 4 ürünü getir
        var relatedProducts = await _context.Products
            .Where(p => p.KategoriId == mainProduct.KategoriId && p.Id != id && p.Aktif)
            .Take(4)
            .Select(p => new ListProductsDTO
            {
                Id = p.Id,
                UrunAdi = p.UrunAdi,
                Fiyat = p.Fiyat,
                Resim = p.Resim,
                KategoriAdi = p.Kategori.KategoriAdi
            })
            .ToListAsync();

        return Ok(relatedProducts);
    }


    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDTO dto)
    {
        var entity = new Product
        {
            UrunAdi = dto.UrunAdi,
            Fiyat = dto.Fiyat,
            Aciklama = dto.Aciklama,
            Anasayfa = dto.Anasayfa,
            KategoriId = dto.KategoriId,
            Aktif = dto.Aktif,
        };
        // --> Resim Upload işlemi <--
        var imageName = await UploadImageAsync(dto.Resim);
        if (imageName != null)
        {
            entity.Resim = imageName;
        }

        _context.Products.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = entity.Id },
            entity
        );
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDTO dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        var product = await _context.Products.FirstOrDefaultAsync(i => i.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        product.UrunAdi = dto.UrunAdi;
        product.Fiyat = dto.Fiyat;
        product.Resim = dto.ResimAdi;
        product.Aciklama = dto.Aciklama;
        product.Aktif = dto.Aktif;
        product.Anasayfa = dto.Anasayfa;
        product.KategoriId = dto.KategoriId;

        if (dto.Resim != null && dto.Resim.Length > 0)
        {
            var imageName = await UploadImageAsync(dto.Resim);
            product.Resim = imageName;
        }
        else
        {
            // Yeni resim seçilmediyse, UI'dan gelen eski resim adını koru
            // Eğer dto.ResimAdi da boşsa eski değeri hiç değiştirme
            if (!string.IsNullOrEmpty(dto.ResimAdi))
            {
                product.Resim = dto.ResimAdi;
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(i => i.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    // --------------> Resim Upload Metodu <------------------
    private async Task<string?> UploadImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";// Guid ile her dosya benzersiz.
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    } //--------------------------------------------------------
}