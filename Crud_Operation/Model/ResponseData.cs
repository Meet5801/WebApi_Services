using System.Globalization;

namespace Crud_Operation.Model
{
    public class ResponseData
    {
        public bool success { get; set; } = false;
        public string message { get; set; } = "something went wrong, please try again later";
        public Object data { get; set; }
        public int code { get; set; } = 500;
    }

    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class Paginationresponse
    {
        public int totalCount { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
