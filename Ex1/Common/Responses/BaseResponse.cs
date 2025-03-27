using System.Net;

namespace Ex1.Common.Responses
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }  
        public string Message { get; set; }  
        public HttpStatusCode StatusCode { get; set; }
    }
}
