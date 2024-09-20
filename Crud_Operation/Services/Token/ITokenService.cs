using Crud_Operation.Model;

namespace Crud_Operation.Services.Token
{
    public interface ITokenService
    {
        string GenerateAuthToken(LoginReponseView loginResponse);
        string GenerateRefreshToken();
        string GetUserIdFromToken(string token);
    }
}
