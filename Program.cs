using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
if (string.IsNullOrWhiteSpace(cosmosConnectionString))
{
    throw new InvalidOperationException("CosmosDbConnectionString is not configured.");
}

var sqlCatalogConnectionString = Environment.GetEnvironmentVariable("SqlCatalogConnectionString");
if (string.IsNullOrWhiteSpace(sqlCatalogConnectionString))
{
    throw new InvalidOperationException("SqlCatalogConnectionString is not configured.");
}

builder.Services.AddSingleton(_ => new CosmosClient(cosmosConnectionString));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
