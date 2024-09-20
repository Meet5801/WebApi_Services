using Crud_Operation.Model;

namespace Crud_Operation.Services.Interface
{
    public interface IAuthService
    {
        Task<User> Register(User model);
        Task<LoginReponseView> Login(LoginViewModel model);
        Task<User> RefreshToken (string token);
        Task UpdateRefreshToken(int userId, string refreshToken);

    }
}
