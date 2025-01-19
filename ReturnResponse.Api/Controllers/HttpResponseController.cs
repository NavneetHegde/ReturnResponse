using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReturnResponse.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReturnResponse.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HttpResponseController : ControllerBase
    {
        private readonly TableClient _tableClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseModelController{T}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration settings.</param>
        private readonly IConfiguration _configuration;

        public HttpResponseController(IConfiguration configuration, TableServiceClient tableServiceClient)
        {
            _configuration = configuration;
            _tableClient = tableServiceClient.GetTableClient("retrunresponsetable");
            _tableClient.CreateIfNotExists();
        }

        /// <summary>
        /// Adds a new HTTP response model to the table storage.
        /// </summary>
        /// <param name="httpResponse">The HTTP response model to add.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HttpResponseModel httpResponse)
        {
            try
            {
                // Create a new table entity from the HTTP response model
                var entity = new TableEntity(string.Empty, httpResponse.Id)
                {
                    { "Id", httpResponse.Id },
                    { "StatusCode", httpResponse.StatusCode },
                    { "ReasonPhrase", httpResponse.ReasonPhrase },
                    { "Timestamp", httpResponse.Timestamp },
                    { "Body", JsonConvert.SerializeObject(httpResponse.Body) },
                    //{ "Headers", JsonConvert.SerializeObject(httpResponse.Headers) },
                    //{ "Cookies", JsonConvert.SerializeObject(httpResponse.Cookies) }
                };

                // Add the entity to the table storage
                await _tableClient.AddEntityAsync(entity);

                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                // If an error occurred, return a 500 Internal Server Error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves an HTTP response model by its ID.
        /// </summary>
        /// <param name="id">The ID of the HTTP response model to retrieve.</param>
        /// <returns>An <see cref="HttpResponseMessage"/> representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var query = _tableClient.QueryAsync<TableEntity>(filter: $"RowKey eq '{id}'");
                await foreach (var entity in query)
                {
                    var httpResponse = new HttpResponseModel
                    {
                        StatusCode = entity.GetInt32("StatusCode") ?? 200,
                        ReasonPhrase = entity.GetString("ReasonPhrase"),
                        Timestamp = entity.GetDateTime("Timestamp").GetValueOrDefault(),
                        Body = entity.GetString("Body"),
                        //Headers = AddHeadersToResponse(entity.GetString("Headers")),
                        //Cookies = (Dictionary<string, string>)entity.GetString("Cookies")
                    };

                    return new ObjectResult(httpResponse.Body)
                    {
                        StatusCode = httpResponse.StatusCode
                    };
                }

                return new ObjectResult("Entity not found")
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
            catch (Exception ex)
            {
                return new ObjectResult("Internal server error")
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        /// <summary>
        /// Updates an existing HTTP response model.
        /// </summary>
        /// <param name="id">The ID of the HTTP response model to update.</param>
        /// <param name="httpResponse">The updated HTTP response model.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] HttpResponseModel httpResponse)
        {
            try
            {
                // Create a new table entity from the HTTP response model
                var entity = new TableEntity(httpResponse.StatusCode.ToString(), id)
                {
                    { "StatusCode", httpResponse.StatusCode },
                    { "ReasonPhrase", httpResponse.ReasonPhrase },
                    { "Timestamp", httpResponse.Timestamp },
                    { "Body", JsonConvert.SerializeObject(httpResponse.Body) },
                    //{ "Headers", JsonConvert.SerializeObject(httpResponse.Headers) },
                    //{ "Cookies", JsonConvert.SerializeObject(httpResponse.Cookies) }
                };

                // Update the entity in the table storage
                await _tableClient.UpsertEntityAsync(entity);
                return Ok(httpResponse);
            }
            catch (Exception ex)
            {
                // If an error occurred, return a 500 Internal Server Error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an HTTP response model by its ID.
        /// </summary>
        /// <param name="id">The ID of the HTTP response model to delete.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                // Query the table storage for the entity with the specified ID
                var query = _tableClient.QueryAsync<TableEntity>(filter: $"RowKey eq '{id}'");
                await foreach (var entity in query)
                {
                    await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
                }

                // Return a 204 No Content response
                return NoContent();
            }
            catch (Exception ex)
            {
                // If an error occurred, return a 500 Internal Server Error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Utility method to add headers
        private void AddHeadersToResponse(IDictionary<string, string> headers)
        {
            if (headers == null || !headers.Any())
                return; // Exit early if the headers dictionary is null or empty

            foreach (var (key, value) in headers)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue; // Skip headers with null, empty, or whitespace keys

                if (!Response.Headers.ContainsKey(key))
                {
                    Response.Headers[key] = value ?? string.Empty; // Add the header, ensuring value is not null
                }
            }
        }
    }
}