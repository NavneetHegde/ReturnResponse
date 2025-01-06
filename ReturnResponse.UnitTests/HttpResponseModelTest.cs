using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace ReturnResponse.Api.Models.Tests
{
    public class HttpResponseModelTest
    {
        [Fact]
        public void HttpResponseModel_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var responseModel = new HttpResponseModel();

            // Assert
            Assert.NotNull(responseModel.Id);
            Assert.Equal(200, responseModel.StatusCode);
            Assert.Equal("OK", responseModel.ReasonPhrase);
            Assert.Equal(DateTime.UtcNow.Date, responseModel.Timestamp.Date);
            Assert.Null(responseModel.Body);
            Assert.Empty(responseModel.Headers);
            Assert.Empty(responseModel.Cookies);
        }

        [Fact]
        public void HttpResponseModel_ShouldInitializeWithProvidedValues()
        {
            // Arrange
            var statusCode = 404;
            var reasonPhrase = "Not Found";
            var body = "Error: Not Found";

            // Act
            var responseModel = new HttpResponseModel(statusCode, reasonPhrase, body);

            // Assert
            Assert.NotNull(responseModel.Id);
            Assert.Equal(statusCode, responseModel.StatusCode);
            Assert.Equal(reasonPhrase, responseModel.ReasonPhrase);
            Assert.Equal(DateTime.UtcNow.Date, responseModel.Timestamp.Date);
            Assert.Equal(body, responseModel.Body);
            Assert.Empty(responseModel.Headers);
            Assert.Empty(responseModel.Cookies);
        }

        [Fact]
        public void ToHttpResponseMessage_ShouldConvertToHttpResponseMessage()
        {
            // Arrange
            var statusCode = 200;
            var reasonPhrase = "OK";
            var body = "Success";
            var headers = new Dictionary<string, string> { { "X-Custom-Header", "HeaderValue" } };
            var cookies = new Dictionary<string, string> { { "SessionId", "12345" } };

            var responseModel = new HttpResponseModel(statusCode, reasonPhrase, body)
            {
                Headers = headers,
                Cookies = cookies
            };

            // Act
            var httpResponseMessage = responseModel.ToHttpResponseMessage();

            // Assert
            Assert.Equal((HttpStatusCode)statusCode, httpResponseMessage.StatusCode);
            var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var expectedContent = JsonConvert.SerializeObject(responseModel);
            Assert.Equal(expectedContent, content);
            Assert.True(httpResponseMessage.Headers.Contains("X-Custom-Header"));
            Assert.True(httpResponseMessage.Headers.Contains("Set-Cookie"));
        }
    }
}