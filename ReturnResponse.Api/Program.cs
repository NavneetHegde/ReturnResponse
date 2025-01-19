using Azure.Data.Tables;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var keyVaultUri = builder.Configuration["KeyVault:Uri"];
var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

// Register TableServiceClient
builder.Services.AddSingleton<TableServiceClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var storageUri = client.GetSecret("storage-uri-secret-name").Value.Value;
    var accountName = client.GetSecret("account-name-secret-name").Value.Value;
    var accountKey = client.GetSecret("account-key-secret-name").Value.Value;
    return new TableServiceClient(new Uri(storageUri), new TableSharedKeyCredential(accountName, accountKey));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();