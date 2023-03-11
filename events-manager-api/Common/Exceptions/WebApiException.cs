using System.Net;

namespace events_manager_api.Common.Exceptions
{
    public class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public object? extraDataError { get; set; }

        public WebApiException(HttpStatusCode statusCode, string message, object? extraDataError = default!) : base(message)
        {
            StatusCode = statusCode;
            this.extraDataError = extraDataError;
        }
    }
}