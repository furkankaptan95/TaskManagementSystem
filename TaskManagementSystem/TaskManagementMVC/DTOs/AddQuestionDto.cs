using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class AddQuestionDto
{
    [Required(ErrorMessage = "İçerik kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [StringLength(200, ErrorMessage = "İçerik maksimum 200 karakter olabilir.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "TaskId gereklidir.")]
    public string TaskId { get; set; }

    [Required(ErrorMessage = "UserId gereklidir.")]
    public string UserId { get; set; }
}
