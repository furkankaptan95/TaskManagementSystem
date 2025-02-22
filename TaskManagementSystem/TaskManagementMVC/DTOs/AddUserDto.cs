using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class AddUserDto
{

    [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz.")]
    [MaxLength(20, ErrorMessage = "Kullanıcı adı en fazla 20 karakter olabilir.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Kullanıcı adı boşluk içeremez.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [MaxLength(100, ErrorMessage = "Email 100 karakterden uzun olamaz.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "İsim kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [MaxLength(50, ErrorMessage = "İsim maksimum 50 karakter olabilir.")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Soyisim kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [MaxLength(50, ErrorMessage = "Soyisim maksimum 50 karakter olabilir.")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakter olabilir.")]
    public string Password { get; set; }

}
