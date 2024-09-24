using OfficeOpenXml.Packaging.Ionic.Zip;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace Crud_Operation.Services.OtpService
{
    public class otpService : IotpService
    {
        private readonly IConfiguration _configuration;
        private readonly string accountSid;
        private readonly string authToken;
        private readonly string verificationCode;
        private readonly string twiliophonenumber;

        public otpService(IConfiguration configuration)
        {
            _configuration = configuration;
            accountSid = _configuration["Twilio:AccountSid"];
            authToken = _configuration["Twilio:AuthToken"];
            verificationCode = _configuration["Twilio:VerificationCode"];
            twiliophonenumber = _configuration["Twilio:TwilioPhonenumber"];
        }
             
        public async Task<VerificationResource> SendOTP(string phoneNumber)
        {
            TwilioClient.Init(accountSid, authToken);

            try
            {
                var verificationResource = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: "sms",
                    pathServiceSid: verificationCode
                );

                return verificationResource;
            }
            catch (ApiException ex)
            {
                // Log the detailed error
                Console.WriteLine($"Twilio API error: {ex.Message} - {ex.Code}");
                throw;
            }
        }

        public async Task<VerificationCheckResource> VerifyOTP(string phoneNumber, string otp)
        {
            TwilioClient.Init(accountSid, authToken);

            try
            {
                var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: otp,
                    pathServiceSid: verificationCode
                );

                return verificationCheckResource;
            }
            catch (ApiException ex)
            {
                // Log the detailed error
                Console.WriteLine($"Twilio API error: {ex.Message} - {ex.Code}");
                throw;
            }
        }
    }
}