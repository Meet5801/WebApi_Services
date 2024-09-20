//using Crud_Operation.Model;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace Crud_Operation.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ConnectionController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public ConnectionController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        protected string GenerateAuthToken(LoginReponseView loginResponse)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_configuration["ApplicationSettings:TokenSecret"]);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//                {
//                    new Claim(ClaimTypes.NameIdentifier, loginResponse.Id.ToString()),
//                }),
//                Expires = DateTime.UtcNow.AddSeconds(1000),
//                Audience = _configuration["ApplicationSettings:Audience"],
//                Issuer = _configuration["ApplicationSettings:Issuer"],
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }

//        protected static string GenerateRefreshToken()
//        {
//            var randomNumber = new byte[64];
//            using var rng = RandomNumberGenerator.Create();
//            rng.GetBytes(randomNumber);
//            return Convert.ToBase64String(randomNumber);
//        }

//        protected string GetUserIdFromToken(string token)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            try
//            {
//                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

//                if (jsonToken == null)
//                {
//                    return null;
//                }

//                foreach (var claim in jsonToken.Claims)
//                {
//                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
//                }

//                var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "nameid");
//                return userIdClaim?.Value;
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//        }
//    }
//}
