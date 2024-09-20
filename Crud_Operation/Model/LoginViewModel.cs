using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Crud_Operation.Model
{
    public class LoginViewModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
    public class LoginReponseView
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string Email { get; set; }
        public string token { get; set; }
        public string refreshtoken { get; set; }

    }
}
