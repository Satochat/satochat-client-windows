using System;
using System.Net;

namespace Satochat.Client.Service.Api {
    public class SatoApiException : Exception {
        public HttpStatusCode StatusCode { get; }

        public SatoApiException() {
        }

        public SatoApiException(string message) : base(message) {
        }

        public SatoApiException(string message, HttpStatusCode statusCode) : base(message) {
            StatusCode = statusCode;
        }

        public SatoApiException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}
