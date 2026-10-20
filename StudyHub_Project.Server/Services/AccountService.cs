using Microsoft.AspNetCore.Identity;

using StudyHub_Project.Server.Data;
using StudyHub_Project.Server.Data.Repositories;
using StudyHub_Project.Server.DTOs.Auth;
using StudyHub_Project.Server.Exceptions;
using StudyHub_Project.Server.Models;

namespace StudyHub_Project.Server.Services;

public class AccountService
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly INoteRepository _noteRepository;
    private readonly IFocusSessionRepository _focusRepository;
    private readonly ICampfireRepository _campfireRepository;

    private readonly InMemoryDataStore _dataStore;

    private readonly IPasswordHasher<UserAccount>
        _passwordHasher;


    public AccountService(
        IUserRepository userRepository,
        ITaskRepository taskRepository,
        INoteRepository noteRepository,
        IFocusSessionRepository focusRepository,
        ICampfireRepository campfireRepository,
        InMemoryDataStore dataStore,
        IPasswordHasher<UserAccount> passwordHasher)
    {
        _userRepository = userRepository;
        _taskRepository = taskRepository;
        _noteRepository = noteRepository;
        _focusRepository = focusRepository;
        _campfireRepository = campfireRepository;
        _dataStore = dataStore;
        _passwordHasher = passwordHasher;
    }


    public UserResponse UpdateAccount(
        int userId,
        UpdateAccountRequest request)
    {
        lock (_dataStore.AuthSyncRoot)
        {
            var user =
                GetRequiredUser(
                    userId);


            var oldEmail =
                user.Email;

            var oldPhone =
                user.Phone;


            var name =
                UserValidation.NormalizeName(
                    request.Name);

            var email =
                UserValidation.NormalizeEmail(
                    request.Email);

            var phone =
                UserValidation.NormalizePhone(
                    request.Phone);


            var contactChanged =
                !string.Equals(
                    user.Email,
                    email,
                    StringComparison.OrdinalIgnoreCase)
                ||
                user.Phone != phone;


            if (contactChanged)
            {
                if (string.IsNullOrWhiteSpace(
                        request.CurrentPassword))
                {
                    throw new ApiException(
                        400,
                        "Введите текущий пароль");
                }


                if (!PasswordMatches(
                        user,
                        request.CurrentPassword))
                {
                    throw new ApiException(
                        400,
                        "Неверный пароль");
                }
            }


            if (_userRepository.EmailExists(
                    email,
                    userId))
            {
                throw new ApiException(
                    409,
                    "Этот e-mail уже используется");
            }


            if (_userRepository.PhoneExists(
                    phone,
                    userId))
            {
                throw new ApiException(
                    409,
                    "Этот номер уже используется");
            }


            user.Name = name;
            user.Email = email;
            user.Phone = phone;


            _userRepository.Update(
                user);


            if (!string.Equals(
                    oldEmail,
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                _dataStore.LoginAttempts.TryRemove(
                    $"email:{oldEmail.ToLowerInvariant()}",
                    out _);
            }


            if (oldPhone != phone)
            {
                _dataStore.LoginAttempts.TryRemove(
                    $"phone:{oldPhone}",
                    out _);
            }


            return ToUserResponse(
                user);
        }
    }


    public void ChangePassword(
        int userId,
        ChangePasswordRequest request)
    {
        lock (_dataStore.AuthSyncRoot)
        {
            var user =
                GetRequiredUser(
                    userId);


            if (!PasswordMatches(
                    user,
                    request.CurrentPassword))
            {
                throw new ApiException(
                    400,
                    "Неверный пароль");
            }


            UserValidation.ValidatePassword(
                request.NewPassword);


            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    request.NewPassword);


            _userRepository.Update(
                user);
        }
    }


    public void DeleteAccount(
        int userId,
        DeleteAccountRequest request)
    {
        lock (_dataStore.AuthSyncRoot)
        {
            var user =
                GetRequiredUser(
                    userId);


            if (!PasswordMatches(
                    user,
                    request.Password))
            {
                throw new ApiException(
                    400,
                    "Неверный пароль");
            }


            lock (_dataStore.TaskCampfireSyncRoot)
            {
                _taskRepository.DeleteAllByUserId(
                    userId);

                _campfireRepository.Delete(
                    userId);
            }


            _noteRepository.DeleteAllByUserId(
                userId);


            lock (_dataStore.FocusSyncRoot)
            {
                _focusRepository.DeleteAllByUserId(
                    userId);
            }


            var tokens =
                _dataStore.Sessions
                    .Where(item =>
                        item.Value == userId)
                    .Select(item =>
                        item.Key)
                    .ToList();


            foreach (var token in tokens)
            {
                _dataStore.Sessions.TryRemove(
                    token,
                    out _);
            }


            _dataStore.LoginAttempts.TryRemove(
                $"email:{user.Email.ToLowerInvariant()}",
                out _);

            _dataStore.LoginAttempts.TryRemove(
                $"phone:{user.Phone}",
                out _);


            if (!_userRepository.Delete(
                    userId))
            {
                throw new ApiException(
                    404,
                    "Аккаунт уже удалён");
            }
        }
    }


    private UserAccount GetRequiredUser(
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


        return user;
    }


    private bool PasswordMatches(
        UserAccount user,
        string password)
    {
        if (string.IsNullOrEmpty(
                password))
        {
            return false;
        }


        return _passwordHasher.VerifyHashedPassword(
                   user,
                   user.PasswordHash,
                   password)
               !=
               PasswordVerificationResult.Failed;
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