using Twilio.Rest.Verify.V2.Service;

namespace Crud_Operation.Services.OtpService
{
    public interface IotpService
    {
        Task<VerificationResource> SendOTP(string phoneNumber);
        Task<VerificationCheckResource> VerifyOTP(string phoneNumber, string otp);
    }
}
