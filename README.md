# Return Response
---
## Project Description
Developed a robust solution enabling development teams or organizations to seamlessly add mocked HTTP responses to a database. These responses can be stored with unique identifiers and easily retrieved to simulate API behavior during testing or demonstrations. The system ensures streamlined integration into testing workflows, improving efficiency and reliability.
## Features
- **Generic Base Controller**: A generic base controller (`HttpResponseModelControllerBase<T>`) for handling HTTP response models.
- **Specific Controllers**: Non-generic controllers that inherit from the generic base controller for specific types of responses.
- **Azure Table Storage**: Integration with Azure Table Storage to store HTTP response models.
- **Swagger Integration**: Swagger (Swashbuckle) integration for API documentation.
- **Unit Tests**: Unit tests using xUnit and Moq to ensure the functionality of the API.

## Installation Instructions

1. Clone the repository:

   ```sh
   git clone https://github.com/NavneetHegde/ReturnResponse.git
   cd ReturnResponse
   ```

2. Restore the dependencies:

   ```sh
   dotnet restore
   ```

3. Update the `appsettings.json` file in the `ReturnResponse.Api` project with your Azure Table Storage credentials:

   ```json
   {
     "AzureTableStorage": {
       "StorageUri": "<your_table_service_uri>",
       "AccountName": "<your_account_name>",
       "AccountKey": "<your_account_key>"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

## Usage

1. Build the solution:

   ```sh
   dotnet build
   ```

2. Run the API project:

   ```sh
   dotnet run --project ReturnResponse.Api
   ```

3. Navigate to `https://localhost:<port>/swagger` to view the Swagger UI and interact with the API.

## API Reference

### Endpoints

- **POST /api/HttpResponse**: Adds a new HTTP response model.
- **GET /api/HttpResponse/{id}**: Retrieves an HTTP response model by ID.
- **PUT /api/HttpResponse/{id}**: Updates an existing HTTP response model.
- **DELETE /api/HttpResponse/{id}**: Deletes an HTTP response model by ID.

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Technologies Used

- .NET 8.0
- Azure Table Storage
- Swagger (Swashbuckle)
- xUnit
- Moq

## Example Code

Here is an example of how to use the API to add a new HTTP response model:

```csharp
var httpClient = new HttpClient();
var response = await httpClient.PostAsJsonAsync("https://localhost:<port>/api/yourcontroller", new HttpResponseModel
{
    StatusCode = 200,
    ReasonPhrase = "OK",
    Timestamp = DateTime.UtcNow,
    Body = "Response body",
    Headers = new Dictionary<string, string> { { "HeaderKey", "HeaderValue" } },
    Cookies = new Dictionary<string, string> { { "CookieKey", "CookieValue" } }
});
```

## Swagger/OpenAPI Documentation

You can view the Swagger UI for this project at `https://localhost:<port>/swagger`.

## Getting Help

If you need help or have any questions, please open an issue on the GitHub repository.

## Additional Notes

- **Known Issues**: List any known issues or bugs.
- **Roadmap**: Outline the future plans or features for the project.
- **Community Resources**: Provide links to any community resources or forums.


Feel free to customize the [`README.md`](command:_github.copilot.openRelativePath?%5B%7B%22scheme%22%3A%22file%22%2C%22authority%22%3A%22%22%2C%22path%22%3A%22%2Fc%3A%2Fcode%2FVSCode%2FReturnResponse%2FREADME.md%22%2C%22query%22%3A%22%22%2C%22fragment%22%3A%22%22%7D%2C%223d9203df-d336-430f-acc9-fd06420fa519%22%5D "c:\code\VSCode\ReturnResponse\README.md") file further based on your specific project details and requirements.