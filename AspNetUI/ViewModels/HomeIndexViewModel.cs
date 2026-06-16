using AspNetUI.DTOs.Categori;
using AspNetUI.DTOs.Product;

namespace AspNetUI.ViewModels;

public class HomeIndexViewModel
{
    public List<ListProductsDTO> Products { get; set; } = new();
    public List<CategoriListDTO> Categories { get; set; } = new();
}