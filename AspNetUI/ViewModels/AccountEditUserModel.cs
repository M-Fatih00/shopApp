using System.ComponentModel.DataAnnotations;

namespace AspNetUI.ViewModels
{
    public class AccountEditUserModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "En fazla 100 karakter olabilir")]
        public string AdSoyad { get; set; } = null!;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
    }
}