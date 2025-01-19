using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ReturnResponse.Api.Controllers;
using ReturnResponse.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReturnResponse.Api.Tests.Controllers
{
    public class HttpResponseModelControllerTest
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<TableClient> _mockTableClient;
        private readonly Mock<TableServiceClient> _mockTableServiceClient;
        private readonly HttpResponseController _controller;

        private readonly string base64String = "U29ycnksIFdvcmxkIQ==";

        public HttpResponseModelControllerTest()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTableServiceClient = new Mock<TableServiceClient>(new Uri("https://example.table.core.windows.net/"), new TableSharedKeyCredential("accountName", base64String));
            _mockTableClient = new Mock<TableClient>();

            _mockConfiguration.SetupGet(x => x["AzureTableStorage:StorageUri"]).Returns("https://example.table.core.windows.net/");
            _mockConfiguration.SetupGet(x => x["AzureTableStorage:AccountName"]).Returns("accountName");
            _mockConfiguration.SetupGet(x => x["AzureTableStorage:AccountKey"]).Returns(base64String);

            _mockTableServiceClient.Setup(x => x.GetTableClient(It.IsAny<string>())).Returns(_mockTableClient.Object);

            _controller = new HttpResponseController(_mockConfiguration.Object, _mockTableServiceClient.Object);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WhenHttpResponseIsValid()
        {
            // Arrange
            var httpResponse = new HttpResponseModel
            {
                StatusCode = 200,
                ReasonPhrase = "OK",
                Timestamp = DateTime.UtcNow,
                Body = "Response body",
                //Headers = new Dictionary<string, string> { { "HeaderKey", "HeaderValue" } },
                //Cookies = new Dictionary<string, string> { { "CookieKey", "CookieValue" } }
            };

            // Act
            var result = await _controller.Post(httpResponse);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<HttpResponseModel>(okResult.Value);
            Assert.Equal(httpResponse.StatusCode, returnValue.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WhenEntityIsFound()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var tableEntity = new TableEntity("200", id)
            {
                { "ReasonPhrase", "OK" },
                { "Timestamp", DateTime.UtcNow },
                { "Body", JsonConvert.SerializeObject("Response body") },
                { "Headers", JsonConvert.SerializeObject(new Dictionary<string, string> { { "HeaderKey", "HeaderValue" } }) },
                { "Cookies", JsonConvert.SerializeObject(new Dictionary<string, string> { { "CookieKey", "CookieValue" } }) }
            };

            var asyncPageable = new Mock<AsyncPageable<TableEntity>>();
            asyncPageable.Setup(p => p.GetAsyncEnumerator(default))
                .Returns(new MockAsyncEnumerator(new List<TableEntity> { tableEntity }));

            _mockTableClient.Setup(tc => tc.QueryAsync<TableEntity>(It.IsAny<string>(), null, null, default))
                .Returns(asyncPageable.Object);

            // Act
            var result = await _controller.Get(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<HttpResponseModel>(okResult.Value);
            Assert.Equal(id, returnValue.Id);
        }

        [Fact]
        public async Task Put_ReturnsOkResult_WhenHttpResponseIsValid()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var httpResponse = new HttpResponseModel
            {
                StatusCode = 200,
                ReasonPhrase = "OK",
                Timestamp = DateTime.UtcNow,
                Body = "Updated response body",
                //Headers = new Dictionary<string, string> { { "HeaderKey", "HeaderValue" } },
                //Cookies = new Dictionary<string, string> { { "CookieKey", "CookieValue" } }
            };

            // Act
            var result = await _controller.Put(id, httpResponse);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<HttpResponseModel>(okResult.Value);
            Assert.Equal(httpResponse.Id, returnValue.Id);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentResult_WhenEntityIsDeleted()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();
            var tableEntity = new TableEntity("200", id);

            var asyncPageable = new Mock<AsyncPageable<TableEntity>>();
            asyncPageable.Setup(p => p.GetAsyncEnumerator(default))
                .Returns(new MockAsyncEnumerator(new List<TableEntity> { tableEntity }));

            _mockTableClient.Setup(tc => tc.QueryAsync<TableEntity>(It.IsAny<string>(), null, null, default))
                .Returns(asyncPageable.Object);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        private class MockAsyncEnumerator : IAsyncEnumerator<TableEntity>
        {
            private readonly IEnumerator<TableEntity> _inner;

            public MockAsyncEnumerator(IEnumerable<TableEntity> entities)
            {
                _inner = entities.GetEnumerator();
            }

            public TableEntity Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }
        }
    }
}