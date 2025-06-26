using System.ComponentModel.DataAnnotations;

namespace FinexaApi.Models.RequestModel
{
    public class RegisterRequestModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z]+\.(com|net|org|edu|gov|mil|co|io|info|biz|me|us|tr)$",
    ErrorMessage = "Geçerli bir e-posta adresi giriniz (örn. example@gmail.com)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; }
    }
}
