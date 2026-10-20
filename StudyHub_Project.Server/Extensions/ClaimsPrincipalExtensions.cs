using StudyHub_Project.Server.Exceptions;
using System.Security.Claims;

namespace StudyHub_Project.Server.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetRequiredUserId(
        this ClaimsPrincipal user)
    {
        var value =
            user.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!int.TryParse(
                value,
                out var userId))
        {
            throw new ApiException(
                401,
                "Войдите в аккаунт");
        }

        return userId;
    }
}