using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Auth;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Введите текущий пароль")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите новый пароль")]
    public string NewPassword { get; set; } = string.Empty;
}