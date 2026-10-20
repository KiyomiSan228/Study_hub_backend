using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using System.Security.Claims;
using System.Text.Encodings.Web;


namespace StudyHub_Project.Server.Authentication;

public class InMemoryBearerAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly InMemoryDataStore _dataStore;
    private readonly IUserRepository _userRepository;

    public InMemoryBearerAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        InMemoryDataStore dataStore,
        IUserRepository userRepository)
        : base(
            options,
            logger,
            encoder)
    {
        _dataStore = dataStore;
        _userRepository = userRepository;
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        var authorizationHeader =
            Request.Headers.Authorization.ToString();




        if (string.IsNullOrWhiteSpace(
                authorizationHeader))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult());
        }

        if (!authorizationHeader.StartsWith(
                "Bearer ",
                StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Некорректный Authorization header."));
        }

        var token =
            authorizationHeader[
                "Bearer ".Length..
            ].Trim();


        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Токен отсутствует."));
        }


        var sessionFound =
            _dataStore.Sessions.TryGetValue(
                token,
                out var userId);


        if (!sessionFound)
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Сессия недействительна."));
        }

        var user =
            _userRepository.GetById(userId);


        if (user is null)
        {

            _dataStore.Sessions.TryRemove(
                token,
                out _);


            return Task.FromResult(
                AuthenticateResult.Fail(
                    "Пользователь не найден."));
        }

        var claims =
            new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Name),

                new Claim(
                    ClaimTypes.Email,
                    user.Email)
            };

        var identity =
            new ClaimsIdentity(
                claims,
                Scheme.Name);


        var principal =
            new ClaimsPrincipal(identity);


        var ticket =
            new AuthenticationTicket(
                principal,
                Scheme.Name);


        return Task.FromResult(
            AuthenticateResult.Success(ticket));
    }


    protected override async Task HandleChallengeAsync(
        AuthenticationProperties properties)
    {
        Response.StatusCode =
            StatusCodes.Status401Unauthorized;

        Response.ContentType =
            "application/json";


        await Response.WriteAsJsonAsync(
            new
            {
                message = "Войдите в аккаунт"
            });
    }

}