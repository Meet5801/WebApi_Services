using Crud_Operation.Model;
using Crud_Operation.Services;
using Crud_Operation.Services.Interface;
using Crud_Operation.Services.OtpService;
using Crud_Operation.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Crud_Operation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ResponseData responseData = new ResponseData();
        private readonly IAuthService _authservice;
        private readonly ITokenService _tokenservice;
        private readonly IotpService _otpService;

        public AuthController(IConfiguration configuration, IAuthService authService, ITokenService tokenService, IotpService otpService)
        {
            _authservice = authService;
            _tokenservice = tokenService;
            _otpService = otpService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ResponseData>> Register(User model)
        {
            var responseData = new ResponseData(); // Initialize responseData

            try
            {
                var user = await _authservice.Register(model);
                if (user != null)
                {
                    responseData.code = 200;
                    responseData.data = user;
                    responseData.message = "Register user successfully";
                    responseData.success = true;
                }
                else
                {
                    responseData.code = 400; // Add code for failure
                    responseData.message = "Problem while registering user!!";
                    responseData.success = false;
                }
            }
            catch (Exception ex)
            {
                responseData.code = 500; // Add code for exception
                responseData.message = "Invalid data entered: " + ex.Message;
                responseData.success = false;
            }

            return responseData.code == 400 || responseData.code == 500
                ? BadRequest(responseData)
                : Ok(responseData);
        }


        [HttpPost("Login")]
        public async Task<ActionResult<ResponseData>> Login(LoginViewModel model)
        {
            try
            {
                var user = await _authservice.Login(model);
                if (user != null)
                {
                    user.token = _tokenservice.GenerateAuthToken(new LoginReponseView { Id = user.Id });
                    user.refreshtoken = _tokenservice.GenerateRefreshToken();

                    await _authservice.UpdateRefreshToken(user.Id, user.refreshtoken);

                    responseData.success = true;
                    responseData.message = "Login successfully";
                    responseData.data = user;
                    responseData.code = 200;
                }
                else
                {
                    responseData.message = "The provided mobile number does not match any user in our records!";
                    responseData.code = 404;
                }

                return new JsonResult(responseData);
            }
            catch (Exception ex)
            {
                responseData.message = "Invalid data entered: " + ex.Message;
                return new JsonResult(responseData);
            }
        }


        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ResponseData>> RefreshToken(string refreshToken)
        {
            var responseData = new ResponseData();
            try
            {
                var user = await _authservice.RefreshToken(refreshToken);
                if (user != null)
                {
                    var newToken = _tokenservice.GenerateAuthToken(new LoginReponseView { Id = user.Id });
                    var newRefreshToken = _tokenservice.GenerateRefreshToken();

                    await _authservice.UpdateRefreshToken(user.Id, newRefreshToken);

                    responseData.success = true;
                    responseData.message = "Token refreshed successfully";
                    responseData.code = 200;
                    responseData.data = new TokenResponse
                    {
                        Token = newToken,
                        RefreshToken = newRefreshToken
                    };
                }
                else
                {
                    responseData.success = false;
                    responseData.message = "Invalid refresh token";
                    responseData.code = 400;
                }
            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = "Error refreshing token: " + ex.Message;
                responseData.code = 500; // Internal server error
            }

            return new JsonResult(responseData);
        }
        [HttpPost("SendOTP")]
        public async Task<ActionResult<ResponseData>> SendOTP(string phoneNumber)
        {
            responseData = new ResponseData(); // Initialize responseData

            try
            {
                var verificationResource = await _otpService.SendOTP(phoneNumber);
                if (verificationResource != null)
                {
                    responseData.code = 200;
                    responseData.message = "OTP sent successfully";
                    responseData.success = true;
                }
                else
                {
                    responseData.code = 400;
                    responseData.message = "Failed to send OTP";
                    responseData.success = false;
                }
            }
            catch (Exception ex)
            {
                responseData.code = 500;
                responseData.message = "Error sending OTP: " + ex.Message;
                responseData.success = false;
            }

            return responseData.code == 400 || responseData.code == 500
                ? BadRequest(responseData)
                : Ok(responseData);
        }

        [HttpPost("VerifyOTP")]
        public async Task<ActionResult<ResponseData>> VerifyOTP(string phoneNumber, string otp)
        {
            responseData = new ResponseData(); // Initialize responseData

            try
            {
                var verificationCheckResource = await _otpService.VerifyOTP(phoneNumber, otp);
                if (verificationCheckResource != null && verificationCheckResource.Status == "approved")
                {
                    responseData.code = 200;
                    responseData.message = "OTP verified successfully";
                    responseData.success = true;
                }
                else
                {
                    responseData.code = 400;
                    responseData.message = "Invalid OTP";
                    responseData.success = false;
                }
            }
            catch (Exception ex)
            {
                responseData.code = 500;
                responseData.message = "Error verifying OTP: " + ex.Message;
                responseData.success = false;
            }

            return responseData.code == 400 || responseData.code == 500
                ? BadRequest(responseData)
                : Ok(responseData);
        }
    }
}
