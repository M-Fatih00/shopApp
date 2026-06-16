namespace AspNetUI.DTOs.Categori;

public class CategoriListDTO
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;
    public int UrunSayisi { get; set; }
}