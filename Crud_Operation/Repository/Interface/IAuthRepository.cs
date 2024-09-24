using Crud_Operation.Model;

namespace Crud_Operation.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<User> Register(User entity);
        Task<LoginReponseView> Login(LoginViewModel entity);
        Task<User>  RefreshToken (string token);
        Task UpdateRefreshToken(int userId, string refreshToken);
        Task<bool> IsPhoneNumberExists(string phoneNumber);
        Task<User> GetUserByPhoneNumber(string phoneNumber); 


    }
}
