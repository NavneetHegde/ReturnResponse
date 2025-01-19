using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ReturnResponse.Api.Models
{
    // <summary>
    /// Represents an HTTP response with various properties such as status code, reason phrase, body, and HTTP details.
    /// </summary>
    public class HttpResponseModel
    {
        
        public string Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the status code of the HTTP response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the reason phrase of the HTTP response.
        /// </summary>        
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when the response was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

        ///// <summary>
        ///// Gets or sets the headers to be included in the HTTP response.
        ///// </summary>
        //public Dictionary<string, string> Headers { get; set; } = new();

        ///// <summary>
        ///// Gets or sets the cookies to be included in the HTTP response.
        ///// </summary>
        //public Dictionary<string, string> Cookies { get; set; } = new();

        /// <summary>
        /// Gets or sets the body of the HTTP response.
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseModel"/> class.
        /// </summary>
        public HttpResponseModel(int statusCode = 200, string reasonPhrase = "OK", object body = default)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Timestamp = DateTime.UtcNow;
            Body = body;
        }

        /// <summary>
        /// Converts the <see cref="HttpResponseModel"/> to an <see cref="HttpResponseMessage"/>.
        /// </summary>
        public HttpResponseMessage ToHttpResponseMessage()
        {
            // Create a new HTTP response message with the status code and body
            var response = new HttpResponseMessage((HttpStatusCode)StatusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json")
            };

            //// Add the headers to the response
            //foreach (var header in Headers)
            //{
            //    response.Headers.Add(header.Key, header.Value);
            //}

            //// Add the cookies to the response
            //foreach (var cookie in Cookies)
            //{
            //    response.Headers.Add("Set-Cookie", $"{cookie.Key}={cookie.Value}");
            //}

            // Return the response
            return response;
        }

        public void Validate()
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), StatusCode))
            {
                throw new ArgumentException("Invalid HTTP status code.");
            }
        }

    }

}
