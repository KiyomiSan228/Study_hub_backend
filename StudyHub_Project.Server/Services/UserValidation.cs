using StudyHub_Project.Server.Exceptions;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace StudyHub_Project.Server.Services;

public static class UserValidation
{

    public const int NameMaxLength = 50;

    public const int PasswordMinLength = 8;

    public static string NormalizeName(
        string name)
    {
        var normalized =
            name.Trim();


        if (string.IsNullOrWhiteSpace(
                normalized))
        {
            throw new ApiException(
                400,
                "Введите имя");
        }


        if (normalized.Length >
            NameMaxLength)
        {
            throw new ApiException(
                400,
                $"Имя не должно быть длиннее {NameMaxLength} символов");
        }


        return normalized;
    }


    public static string NormalizeEmail(
        string email)
    {
        var normalized =
            email
                .Trim()
                .ToLowerInvariant();


        if (string.IsNullOrWhiteSpace(
                normalized))
        {
            throw new ApiException(
                400,
                "Введите e-mail");
        }


        if (!MailAddress.TryCreate(
                normalized,
                out _))
        {
            throw new ApiException(
                400,
                "Укажите корректный e-mail");
        }


        return normalized;
    }

    public static string NormalizePhone(
        string phone)
    {
        if (string.IsNullOrWhiteSpace(
                phone))
        {
            throw new ApiException(
                400,
                "Введите номер телефона");
        }

        var normalized =
            Regex.Replace(
                phone.Trim(),
                @"[^\d+]",
                "");

        if (Regex.IsMatch(
                normalized,
                @"^8\d{10}$"))
        {
            normalized =
                "+7" + normalized[1..];
        }

        if (Regex.IsMatch(
                normalized,
                @"^7\d{10}$"))
        {
            normalized =
                "+" + normalized;
        }

        if (!Regex.IsMatch(
                normalized,
                @"^\+7\d{10}$"))
        {
            throw new ApiException(
                400,
                "Укажите номер телефона в формате +7XXXXXXXXXX");
        }


        return normalized;
    }


    public static void ValidatePassword(
        string password)
    {
        if (string.IsNullOrEmpty(
                password))
        {
            throw new ApiException(
                400,
                "Введите пароль");
        }


        if (password.Length <
            PasswordMinLength)
        {
            throw new ApiException(
                400,
                $"Пароль должен содержать минимум {PasswordMinLength} символов");
        }


        var hasLetter =
            password.Any(
                char.IsLetter);


        if (!hasLetter)
        {
            throw new ApiException(
                400,
                "Пароль должен содержать хотя бы одну букву");
        }


        var hasDigit =
            password.Any(
                char.IsDigit);


        if (!hasDigit)
        {
            throw new ApiException(
                400,
                "Пароль должен содержать хотя бы одну цифру");
        }
    }
}