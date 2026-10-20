using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Введите имя")]
    [MaxLength(50, ErrorMessage = "Имя слишком длинное")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите e-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите номер телефона")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;
}