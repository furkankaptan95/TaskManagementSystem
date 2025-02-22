using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class NewPasswordDto
{
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
    [MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
    [MaxLength(15, ErrorMessage = "Şifre en fazla 15 karakter olabilir.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Şifre tekrar alanı boş bırakılamaz.")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}
