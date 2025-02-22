using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class ReplyQuestionDto
{
    [Required(ErrorMessage = "Yanıt kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [StringLength(200, ErrorMessage = "Yanıt maksimum 200 karakter olabilir.")]
    public string Reply { get; set; }

    [Required(ErrorMessage = "QuestionId gereklidir.")]
    public string QuestionId { get; set; }
}
