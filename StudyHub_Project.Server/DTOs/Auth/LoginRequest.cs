using StudyHub_Project.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Укажите способ входа")]
    public LoginMethod? Method { get; set; }

    [Required(ErrorMessage = "Введите данные для входа")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;
}