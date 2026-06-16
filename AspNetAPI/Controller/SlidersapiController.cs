using AspNetAPI.Data;
using AspNetAPI.DTO;
using AspNetAPI.DTO.Slider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Controller;


[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SlidersapiController : ControllerBase
{
    private readonly DataContext _context;
    public SlidersapiController(DataContext context)
    {
        _context = context;
    }



    [HttpGet("all")]
    [Authorize(Roles = "Admin")] // Sadece admin her şeyi (aktif/pasif) görebilir
    public async Task<ActionResult> GetAllSliders()
    {
        var sliders = await _context.Sliders
            .OrderBy(i => i.Index)
            .Select(i => SliderToDTO(i))
            .ToListAsync();

        return Ok(sliders);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult> GetSlider(int? id)
    {
        if (id == null)
            return NotFound();

        var slider = await _context.Sliders.Where(i => i.Id == id).Select(s => SliderToDTO(s)).FirstOrDefaultAsync();

        if (slider == null)
            return NotFound();

        return Ok(slider);
    }


    [HttpGet("active")] // api/Slidersapi/active"
    [AllowAnonymous]
    public IActionResult GetSlidersToUI()
    {
        var sliders = _context.Sliders
            .Where(i => i.Aktif)
            .OrderBy(i => i.Index)
            .Select(i => new ListSlidersDTO
            {
                Id = i.Id,
                Baslik = i.Baslik,
                Resim = i.Resim,
                Index = i.Index,
                Aktif = i.Aktif
            })
            .ToList();

        return Ok(sliders);
    }


    [HttpPost]
    public async Task<IActionResult> CreateSlider([FromForm] CreateSliderDTO dto)
    {
        var slider = new Slider
        {
            Baslik = dto.Baslik,
            Index = dto.Index,
            Aktif = dto.Aktif
        };

        // Resim upload
        var imageName = await UploadImageAsync(dto.Resim);
        if (imageName != null)
            slider.Resim = imageName;

        _context.Sliders.Add(slider);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetSlider),
            new { id = slider.Id },
            SliderToDTO(slider)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditSlider(int id, [FromForm] EditSliderDTO dto)
    {

        var slider = await _context.Sliders.FirstOrDefaultAsync(i => i.Id == id);
        if (slider == null) return NotFound();

        slider.Baslik = dto.Baslik;
        slider.Aciklama = dto.Aciklama;
        slider.Index = dto.Index;
        slider.Aktif = dto.Aktif;

        if (dto.Resim != null)
        {
            var oldImage = slider.Resim;
            var imageName = await UploadImageAsync(dto.Resim);
            slider.Resim = imageName!;

            if (!string.IsNullOrEmpty(oldImage))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", oldImage);
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSlider(int id)
    {
        var slider = await _context.Sliders.FirstOrDefaultAsync(i => i.Id == id);

        if (slider == null)
            return NotFound();

        _context.Sliders.Remove(slider);
        await _context.SaveChangesAsync();

        return NoContent();
    }



    private static ListSlidersDTO SliderToDTO(Slider s)
    {
        var dto = new ListSlidersDTO();

        if (s != null)
        {
            dto.Id = s.Id;
            dto.Baslik = s.Baslik;
            dto.Resim = s.Resim;
            dto.Index = s.Index;
            dto.Aktif = s.Aktif;
        }

        return dto;
    }


    // --------------> Resim Upload Metodu <------------------
    private async Task<string?> UploadImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";

        var uploadFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images"
        );

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var path = Path.Combine(uploadFolder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileName;
    }
    // -------------------------------------------------------
}