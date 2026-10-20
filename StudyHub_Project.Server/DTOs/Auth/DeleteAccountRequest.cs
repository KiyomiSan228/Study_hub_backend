using System.ComponentModel.DataAnnotations;

namespace StudyHub_Project.Server.DTOs.Auth;

public class DeleteAccountRequest
{
    [Required(ErrorMessage = "Введите пароль")]
    public string Password { get; set; } = string.Empty;
}