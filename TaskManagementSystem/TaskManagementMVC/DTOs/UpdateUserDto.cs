using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class UpdateUserDto
{
    [Required(ErrorMessage = "Id gereklidir.")]
    public string Id { get; set; }

    [Required(ErrorMessage = "İsim kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [MaxLength(50, ErrorMessage = "İsim maksimum 50 karakter olabilir.")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Soyisim kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [MaxLength(50, ErrorMessage = "Soyisim maksimum 50 karakter olabilir.")]
    public string Lastname { get; set; }
}
