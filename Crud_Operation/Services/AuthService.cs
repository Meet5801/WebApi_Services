using Azure;
using Crud_Operation.Model;
using Crud_Operation.Repository.Interface;
using Crud_Operation.Services.Interface;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<User> GetUserByPhoneNumber(string phoneNumber)
    {
        return await _authRepository.GetUserByPhoneNumber(phoneNumber);
    }

    public async Task<bool> IsPhoneNumberExists(string phoneNumber)
    {
        return await _authRepository.IsPhoneNumberExists(phoneNumber);
    }

    public async Task<LoginReponseView> Login(LoginViewModel model)
    {
        var user = await _authRepository.Login(model);
        if(user == null)
        {
            throw new InvalidOperationException("Invalid Phonenumber Or PassWord!!.");
        }
        return new LoginReponseView
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            
        };
    }

    public async Task<User> RefreshToken(string token)
    {
        return await _authRepository.RefreshToken(token);
    }

    public async Task<User> Register(User model)
    {
        return await _authRepository.Register(model);
    }

    public async Task UpdateRefreshToken(int userId, string refreshToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be greater than zero.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        }
        await _authRepository.UpdateRefreshToken(userId, refreshToken);
    }
}
