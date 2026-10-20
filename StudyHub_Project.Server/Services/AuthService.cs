using Microsoft.AspNetCore.Identity;
using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.DTOs.Auth;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;
using System.Security.Cryptography;


namespace StudyHub_Project.Server.Services;

public class AuthService
{
    private const int MaxLoginFails = 5;
    private static readonly TimeSpan LoginLockDuration =
        TimeSpan.FromMinutes(15);


    private readonly IUserRepository _userRepository;

    private readonly ICampfireRepository _campfireRepository;

    private readonly InMemoryDataStore _dataStore;

    private readonly IPasswordHasher<UserAccount> _passwordHasher;



    public AuthService(
        IUserRepository userRepository,
        ICampfireRepository campfireRepository,
        InMemoryDataStore dataStore,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _userRepository = userRepository;

        _campfireRepository = campfireRepository;

        _dataStore = dataStore;

        _passwordHasher = passwordHasher;
    }


    public AuthResponse Register(
        RegisterRequest request)
    {
        var name =
            UserValidation.NormalizeName(
                request.Name);


        var email =
            UserValidation.NormalizeEmail(
                request.Email);


        var phone =
            UserValidation.NormalizePhone(
                request.Phone);


        UserValidation.ValidatePassword(
            request.Password);


        UserAccount user;


        lock (_dataStore.AuthSyncRoot)
        {

            if (_userRepository.EmailExists(email))
            {
                throw new ApiException(
                    409,
                    "Этот e-mail уже используется");
            }


            if (_userRepository.PhoneExists(phone))
            {
                throw new ApiException(
                    409,
                    "Этот номер телефона уже используется");
            }


            user = new UserAccount
            {
                Name = name,
                Email = email,
                Phone = phone
            };


            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.Password);


            _userRepository.Add(user);


            var campfire = new Campfire
            {
                UserId = user.Id,
                Logs = 0,
                UpdatedAt = DateTimeOffset.UtcNow
            };


            _campfireRepository.Save(
                campfire);
        }


        var token =
            CreateSession(
                user.Id);

        return new AuthResponse
        {
            Token = token,

            User = ToUserResponse(
                user)
        };
    }

    public AuthResponse Login(
        LoginRequest request)
    {

        if (request.Method is null)
        {
            throw new ApiException(
                400,
                "Укажите способ входа");
        }


        if (string.IsNullOrWhiteSpace(
                request.Password))
        {
            throw new ApiException(
                400,
                "Введите пароль");
        }


        var method =
            request.Method.Value;

        var login =
            NormalizeLogin(
                method,
                request.Login);


        var attemptKey =
            CreateLoginAttemptKey(
                method,
                login);


        UserAccount? user;

        lock (_dataStore.AuthSyncRoot)
        {
            var now =
                DateTimeOffset.UtcNow;


            if (_dataStore.LoginAttempts.TryGetValue(
                    attemptKey,
                    out var existingAttempts))
            {
                if (existingAttempts.LockedUntil is not null &&
                    existingAttempts.LockedUntil > now)
                {
                    throw new ApiException(
                        423,
                        "Слишком много попыток, попробуйте через 15 минут");
                }

                if (existingAttempts.LockedUntil is not null &&
                    existingAttempts.LockedUntil <= now)
                {
                    _dataStore.LoginAttempts.TryRemove(
                        attemptKey,
                        out _);
                }
            }


            user =
                FindUserByLogin(
                    method,
                    login);

            var passwordIsCorrect =
                false;


            if (user is not null)
            {
                var verificationResult =
                    _passwordHasher.VerifyHashedPassword(
                        user,
                        user.PasswordHash,
                        request.Password);


                passwordIsCorrect =
                    verificationResult !=
                    PasswordVerificationResult.Failed;


                if (verificationResult ==
                    PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.PasswordHash =
                        _passwordHasher.HashPassword(
                            user,
                            request.Password);


                    _userRepository.Update(
                        user);
                }
            }

            if (user is null ||
                !passwordIsCorrect)
            {
                RegisterFailedLoginAttempt(
                    attemptKey,
                    now);


                throw new ApiException(
                    400,
                    "Неправильные данные для входа");
            }

            _dataStore.LoginAttempts.TryRemove(
                attemptKey,
                out _);
        }


        var token =
            CreateSession(
                user.Id);


        return new AuthResponse
        {
            Token = token,

            User = ToUserResponse(
                user)
        };
    }

    public UserResponse GetCurrentUser(
        int userId)
    {
        var user =
            _userRepository.GetById(
                userId);


        if (user is null)
        {
            throw new ApiException(
                401,
                "Сессия недействительна");
        }


        return ToUserResponse(
            user);
    }

    public void Logout(
        string token)
    {
        if (string.IsNullOrWhiteSpace(
                token))
        {
            return;
        }


        _dataStore.Sessions.TryRemove(
            token,
            out _);
    }

    private string CreateSession(
        int userId)
    {
        while (true)
        {

            var randomBytes =
                RandomNumberGenerator.GetBytes(
                    32);

            var token =
                Convert
                    .ToHexString(randomBytes)
                    .ToLowerInvariant();


            var added =
                _dataStore.Sessions.TryAdd(
                    token,
                    userId);


            if (added)
            {
                return token;
            }
        }
    }

    private UserAccount? FindUserByLogin(
        LoginMethod method,
        string login)
    {
        return method switch
        {
            LoginMethod.Email =>
                _userRepository.GetByEmail(
                    login),

            LoginMethod.Phone =>
                _userRepository.GetByPhone(
                    login),

            _ =>
                throw new ApiException(
                    400,
                    "Некорректный способ входа")
        };
    }

    private static string NormalizeLogin(
        LoginMethod method,
        string login)
    {
        return method switch
        {
            LoginMethod.Email =>
                UserValidation.NormalizeEmail(
                    login),

            LoginMethod.Phone =>
                UserValidation.NormalizePhone(
                    login),

            _ =>
                throw new ApiException(
                    400,
                    "Некорректный способ входа")
        };
    }

    private static string CreateLoginAttemptKey(
        LoginMethod method,
        string login)
    {
        return method switch
        {
            LoginMethod.Email =>
                $"email:{login}",

            LoginMethod.Phone =>
                $"phone:{login}",

            _ =>
                throw new ApiException(
                    400,
                    "Некорректный способ входа")
        };
    }

    private void RegisterFailedLoginAttempt(
        string attemptKey,
        DateTimeOffset now)
    {
        var attempts =
            _dataStore.LoginAttempts.GetOrAdd(
                attemptKey,
                _ => new LoginAttemptState());


        attempts.Fails += 1;


        if (attempts.Fails <
            MaxLoginFails)
        {
            return;
        }


        attempts.Fails = 0;

        attempts.LockedUntil =
            now.Add(
                LoginLockDuration);


        throw new ApiException(
            423,
            "Слишком много попыток, попробуйте через 15 минут");
    }


    private static UserResponse ToUserResponse(
        UserAccount user)
    {
        return new UserResponse
        {
            Id = user.Id,

            Name = user.Name,

            Email = user.Email,

            Phone = user.Phone
        };
    }
}