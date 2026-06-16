
using AspNetAPI.Data;
using AspNetAPI.DTO;

namespace API.Extensions;

public static class OrderExtensions
{
    public static IQueryable<OrderDTO> OrderToDTO(this IQueryable<Order> query)
    {
        return query.Select(i => new OrderDTO
        {
            Id = i.Id,
            Username = i.Username,
            AdSoyad = i.AdSoyad,
            Telefon = i.Telefon,
            AdresSatiri = i.AdresSatiri,
            Sehir = i.Sehir,
            TeslimatUcreti = i.TeslimatUcreti,
            AraToplam = i.AraToplam,
            SiparisTarihi = i.SiparisTarihi,
            OrderItems = i.OrderItems.Select(item => new OrderItemDTO
            {
                Id = item.Id,
                UrunAdi = item.UrunAdi,
                UrunId = item.UrunId,
                UrunResmi = item.UrunResmi,
                Fiyat = item.Fiyat,
                Miktar = item.Miktar
            }).ToList()
        });
    }
}
