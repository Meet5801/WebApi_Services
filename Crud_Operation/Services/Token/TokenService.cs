using Crud_Operation.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Crud_Operation.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAuthToken(LoginReponseView loginResponse)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["ApplicationSettings:TokenSecret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginResponse.Id.ToString()),
                }),
                Expires = DateTime.UtcNow.AddSeconds(60),
                Audience = _configuration["ApplicationSettings:Audience"],
                Issuer = _configuration["ApplicationSettings:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return null;
                }

                foreach (var claim in jsonToken.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid");
                return userIdClaim?.Value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
