using AspNetAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetAPI.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Categori> Categories { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Slider>().HasData(
            new List<Slider>
            {
                new Slider
                {
                    Id=1, 
                    Baslik = "Slider 1 Başlık", 
                    Aciklama = "Slider 1 Açıklama", 
                    Resim = "slider-1.jpeg",
                    Aktif = true,
                    Index=0
                },
                new Slider
                {
                    Id=2, 
                    Baslik = "Slider 2 Başlık", 
                    Aciklama = "Slider 2 Açıklama", 
                    Resim = "slider-2.jpeg",
                    Aktif = true,
                    Index=1
                },
                new Slider
                {
                    Id=3, 
                    Baslik = "Slider 3 Başlık", 
                    Aciklama = "Slider 3 Açıklama", 
                    Resim = "slider-3.jpeg",
                    Aktif = true,
                    Index=2
                }
            }
        );

        modelBuilder.Entity<Categori>().HasData(
            new List<Categori>
            {
                new Categori {Id=1, KategoriAdi="Telefon", Url="telefon"},
                new Categori {Id=2, KategoriAdi="Elektronik", Url="elektronik"},
                new Categori {Id=3, KategoriAdi="Beyaz Eşya", Url="beyaz-esya"},
                new Categori {Id=4, KategoriAdi="Giyim", Url="giyim"},
                new Categori {Id=5, KategoriAdi="Kozmetik", Url="kozmetik"},
                new Categori {Id=6, KategoriAdi="Kategori 1", Url="kategori-1"},
                new Categori {Id=7, KategoriAdi="Kategori 2", Url="kategori-2"},
                new Categori {Id=8, KategoriAdi="Kategori 3", Url="kategori-3"},
                new Categori {Id=9, KategoriAdi="Kategori 4", Url="kategori-4"},
                new Categori {Id=10, KategoriAdi="Kategori 5", Url="kategori-5"}
            }
        );

        modelBuilder.Entity<Product>().HasData(
            new List<Product>()
            {
               new Product() {
                Id = 1,
                UrunAdi = "Apple Watch 7",
                Fiyat = 10000,
                Aktif = false,
                Resim="1.jpeg",
                Anasayfa = true,
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 1},

               new Product() {
                Id = 2,
                UrunAdi = "Apple Watch 8",
                Fiyat = 20000,
                Aktif = true,
                Resim="2.jpeg",
                Anasayfa = true,
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 1},

               new Product() {
                Id = 3, 
                UrunAdi = "Apple Watch 9", 
                Fiyat = 30000, 
                Aktif = true, 
                Resim="3.jpeg", 
                Anasayfa = true, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 2},
                
               new Product() {
                Id = 4, 
                UrunAdi = "Apple Watch 10", 
                Fiyat = 40000, 
                Aktif = false, 
                Resim="4.jpeg", 
                Anasayfa = false, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 2},

               new Product() {
                Id = 5, 
                UrunAdi = "Apple Watch 11", 
                Fiyat = 50000, 
                Aktif = true, 
                Resim="5.jpeg", 
                Anasayfa = true, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 2},

               new Product() {
                Id = 6, 
                UrunAdi = "Apple Watch 12", 
                Fiyat = 60000, 
                Aktif = false, 
                Resim="6.jpeg", 
                Anasayfa = false, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 3},

               new Product() {
                Id = 7, 
                UrunAdi = "Apple Watch 13", 
                Fiyat = 70000, 
                Aktif = false, 
                Resim="7.jpeg", 
                Anasayfa = false, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 3},

               new Product() {
                Id = 8, 
                UrunAdi = "Apple Watch 14", 
                Fiyat = 80000, 
                Aktif = true, 
                Resim="8.jpeg", 
                Anasayfa = true, 
                Aciklama="Lorem ipsum dolor sit amet consectetur adipisicing elit. Culpa cumque odit minima esse eius blanditiis deleniti possimus qui impedit ut! Velit blanditiis atque exercitationem excepturi totam error dolore vel necessitatibus.",
                KategoriId = 4},
            }
        );
    }


}
