using System.ComponentModel.DataAnnotations;

namespace TaskManagementMVC.DTOs;
public class UpdateRoleDto
{
    [Required(ErrorMessage = "UserId gereklidir.")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Rol kısmı boş olamaz.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Boşluklardan oluşan bir değer girilemez.")]
    [StringLength(15, ErrorMessage = "Rol maksimum 15 karakter olabilir.")]
    public string Role { get; set; }
}
