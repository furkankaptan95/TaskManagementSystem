using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class UpdateTaskDto
{
    [Required(ErrorMessage = "Id gereklidir.")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(50, ErrorMessage = "Başlık en fazla 50 karakter olabilir.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Başlık boş olamaz ve sadece boşluklardan oluşamaz.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [MaxLength(300, ErrorMessage = "Açıklama en fazla 300 karakter olabilir.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Açıklama boş olamaz ve sadece boşluklardan oluşamaz.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
    [DataType(DataType.Date, ErrorMessage = "Geçerli bir tarih giriniz.")]
    public DateTime EndDate { get; set; }
}
