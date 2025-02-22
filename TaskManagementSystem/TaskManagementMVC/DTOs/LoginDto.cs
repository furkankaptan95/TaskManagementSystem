using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class LoginDto
{
    [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
    [MaxLength(100, ErrorMessage = "Email 100 karakterden uzun olamaz.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakter olabilir.")]
    public string Password { get; set; }
}
