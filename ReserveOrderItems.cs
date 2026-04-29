using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using Azure.Storage.Blobs;

namespace eShopWeb.Function;

public class ReserveOrderItems
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger _logger;

    public ReserveOrderItems(BlobServiceClient blobServiceClient, ILoggerFactory loggerFactory)
    {
        _blobServiceClient = blobServiceClient;
        _logger = loggerFactory.CreateLogger<ReserveOrderItems>();
    }

    [Function("ReserveOrderItems")]
    public async Task Run(
        [ServiceBusTrigger("order-items-reservation", Connection = "ServiceBusConnection")]
        string message)
    {
        _logger.LogInformation("Processing message...");

        var container = _blobServiceClient.GetBlobContainerClient("orders");

        var fileName = $"order-{Guid.NewGuid()}.json";
        var blob = container.GetBlobClient(fileName);

        int retries = 0;
        bool success = false;

        while (retries < 3 && !success)
        {
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                await blob.UploadAsync(stream, overwrite: true);

                success = true;
            }
            catch (Exception ex)
            {
                retries++;
                _logger.LogError($"Retry {retries}: {ex.Message}");

                if (retries == 3)
                {
                    throw;
                }
            }
        }
    }
}