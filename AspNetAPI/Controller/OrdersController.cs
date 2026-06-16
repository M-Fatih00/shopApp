using System.Security.Claims;
using API.Extensions;
using AspNetAPI.Data;
using AspNetAPI.DTO;
using AspNetAPI.Services;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Controller;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public OrdersController(
        DataContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }


    [HttpGet("GetOrders")]
    public async Task<ActionResult<List<OrderDTO>>> GetOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var orders = await _context.Orders
            .Where(i => i.CustomerId == userId)
            .Include(i => i.OrderItems)
            .OrderToDTO()
            .ToListAsync();

        return Ok(orders);
    }


    [HttpGet("{id}", Name = "GetOrder")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = await _context.Orders
            .Where(i => i.CustomerId == userId && i.Id == id)
            .Include(i => i.OrderItems)
            .OrderToDTO()
            .FirstOrDefaultAsync();

        if (order == null)
            return NotFound(new ProblemDetails
            {
                Title = "Sipariş bulunamadı"
            });

        return Ok(order);
    }

    [HttpGet("GetRecentOrders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<OrderDTO>>> GetRecentOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.SiparisTarihi) // en yeni siparişler
            .Take(5) // son 5 sipariş
            .OrderToDTO()
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("GetAllOrders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<OrderDTO>>> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.SiparisTarihi)
            .OrderToDTO()
            .ToListAsync();

        return Ok(orders);
    }


    [HttpPost("create")]
    public async Task<ActionResult<OrderDTO>> CreateOrder(CreateOrderDTO orderDTO)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var cart = await _context.Carts
            .Include(i => i.CartItems)
            .ThenInclude(i => i.Urun)
            .FirstOrDefaultAsync(i => i.CustomerId == userId);

        if (cart == null)
            return BadRequest(new ProblemDetails { Title = "Sepetiniz bulunamadı" });

        var items = new List<AspNetAPI.Data.OrderItem>();

        foreach (var item in cart.CartItems)
        {
            var urun = await _context.Products.FindAsync(item.UrunId);

            if (urun == null) continue;

            items.Add(new AspNetAPI.Data.OrderItem
            {
                UrunId = urun.Id,
                UrunAdi = urun.UrunAdi!,
                UrunResmi = urun.Resim!,
                Fiyat = urun.Fiyat,
                Miktar = item.Miktar
            });

            urun.Stok -= item.Miktar;
        }

        var order = new Order
        {
            SiparisTarihi = DateTime.UtcNow,
            CustomerId = userId!,
            Username = User.Identity!.Name!,
            AdSoyad = orderDTO.AdSoyad,
            Telefon = orderDTO.Telefon,
            Sehir = orderDTO.Sehir,
            AdresSatiri = orderDTO.AdresSatiri,
            PostaKodu = orderDTO.PostaKodu,
            SiparisNotu = orderDTO.SiparisNotu ?? "",
            AraToplam = items.Sum(i => i.Fiyat * i.Miktar),
            TeslimatUcreti = 0,
            OrderItems = items
        };

        var payment = await ProcessPayment();

        if (payment.Status != "success")
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Ödeme başarısız",
                Detail = payment.ErrorMessage
            });
        }
        _context.Orders.Add(order);
        _context.Carts.Remove(cart);

        await _context.SaveChangesAsync();

        var orderDto = await _context.Orders
                        .Where(i => i.Id == order.Id)
                        .OrderToDTO()
                        .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
    }


    private async Task<Payment> ProcessPayment()
    {
        Options options = new Options();
        options.ApiKey = _configuration["PaymentAPI:APIKey"];
        options.SecretKey = _configuration["PaymentAPI:SecretKey"];
        options.BaseUrl = "https://sandbox-api.iyzipay.com";

        CreatePaymentRequest request = new CreatePaymentRequest();
        request.Locale = Locale.TR.ToString();
        request.ConversationId = "123456789";
        request.Price = "1";
        request.PaidPrice = "1.2";
        request.Currency = Currency.TRY.ToString();
        request.Installment = 1;
        request.BasketId = "B67832";
        request.PaymentChannel = PaymentChannel.WEB.ToString();
        request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

        PaymentCard paymentCard = new PaymentCard();
        paymentCard.CardHolderName = "John Doe";
        paymentCard.CardNumber = "5528790000000008";
        paymentCard.ExpireMonth = "12";
        paymentCard.ExpireYear = "2030";
        paymentCard.Cvc = "123";
        paymentCard.RegisterCard = 0;
        request.PaymentCard = paymentCard;

        Buyer buyer = new Buyer();
        buyer.Id = "BY789";
        buyer.Name = "John";
        buyer.Surname = "Doe";
        buyer.GsmNumber = "+905350000000";
        buyer.Email = "email@email.com";
        buyer.IdentityNumber = "74300864791";
        buyer.LastLoginDate = "2015-10-05 12:43:35";
        buyer.RegistrationDate = "2013-04-21 15:12:09";
        buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        buyer.Ip = "85.34.78.112";
        buyer.City = "Istanbul";
        buyer.Country = "Turkey";
        buyer.ZipCode = "34732";
        request.Buyer = buyer;

        Address shippingAddress = new Address();
        shippingAddress.ContactName = "Jane Doe";
        shippingAddress.City = "Istanbul";
        shippingAddress.Country = "Turkey";
        shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        shippingAddress.ZipCode = "34742";
        request.ShippingAddress = shippingAddress;

        Address billingAddress = new Address();
        billingAddress.ContactName = "Jane Doe";
        billingAddress.City = "Istanbul";
        billingAddress.Country = "Turkey";
        billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
        billingAddress.ZipCode = "34742";
        request.BillingAddress = billingAddress;

        List<BasketItem> basketItems = new List<BasketItem>();
        BasketItem firstBasketItem = new BasketItem();
        firstBasketItem.Id = "BI101";
        firstBasketItem.Name = "Binocular";
        firstBasketItem.Category1 = "Collectibles";
        firstBasketItem.Category2 = "Accessories";
        firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
        firstBasketItem.Price = "0.3";
        basketItems.Add(firstBasketItem);

        BasketItem secondBasketItem = new BasketItem();
        secondBasketItem.Id = "BI102";
        secondBasketItem.Name = "Game code";
        secondBasketItem.Category1 = "Game";
        secondBasketItem.Category2 = "Online Game Items";
        secondBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
        secondBasketItem.Price = "0.5";
        basketItems.Add(secondBasketItem);

        BasketItem thirdBasketItem = new BasketItem();
        thirdBasketItem.Id = "BI103";
        thirdBasketItem.Name = "Usb";
        thirdBasketItem.Category1 = "Electronics";
        thirdBasketItem.Category2 = "Usb / Cable";
        thirdBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
        thirdBasketItem.Price = "0.2";
        basketItems.Add(thirdBasketItem);
        request.BasketItems = basketItems;

        return await Payment.Create(request, options);
    }


}