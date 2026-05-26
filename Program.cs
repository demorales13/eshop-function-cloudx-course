using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Storage.Blobs;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var blobConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnection");

if (string.IsNullOrWhiteSpace(blobConnectionString))
{
    throw new InvalidOperationException("BlobStorageConnection is not configured.");
}

builder.Services.AddSingleton(_ =>
    new BlobServiceClient(blobConnectionString));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
