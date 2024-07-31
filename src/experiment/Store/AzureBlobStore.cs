using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blob.Api.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Blob.Api.Store;
public class AzureBlobStore : IStore
{
    readonly BlobServiceClient store;
    readonly string accountUri;
    public string Root { get; }
    

    public AzureBlobStore(string root)
    {
        Root = root;
        accountUri = $"https://{Root}.blob.core.windows.net";

        store = new(
            new Uri(accountUri),
            new DefaultAzureCredential()
        );
    }

    public async Task<ApiMessage<StoreFile>> Delete(string container, string file)
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);

        if (await ctr.ExistsAsync())
        {
            BlobClient blob = ctr.GetBlobClient(file);

            if (await blob.ExistsAsync())
            {
                StoreFile result = FromBlobClient(blob);
                await blob.DeleteAsync();

                return new(result, $"Blob '{file}' was deleted from container '{container}'");
            }
            else
                return new("Delete", $"Blob '{file}' was not found in container '{container}'");
        }
        else
            return new("Delete", $"Container '{container}' was not found");
    }

    public async Task<FileContentResult> Download(string container, string file)
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);

        if (await ctr.ExistsAsync())
        {
            BlobClient blob = ctr.GetBlobClient(file);

            if (await blob.ExistsAsync())
            {
                BlobDownloadResult result = await blob.DownloadContentAsync();

                return new FileContentResult(
                    result.Content.ToArray(),
                    result.Details.ContentType
                )
                {
                    FileDownloadName = file
                };
            }
            else
                throw new ArgumentException($"File '{file}' was not found in container '{container}'");
        }
        else
            throw new ArgumentException($"Container '{container}' was not found");
    }

    public async Task<List<StoreContainer>> GetContainers()
    {
        List<StoreContainer> containers = [];

        await foreach (BlobContainerItem container in store.GetBlobContainersAsync())
            containers.Add(new() { Name = container.Name });

        return containers;
    }

    public async Task<StoreContainer?> GetContainer(string container)
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);

        return await ctr.ExistsAsync()
            ? new() { Name = container }
            : null;
    }

    public async Task<List<StoreFile>> GetFiles(string container)
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);
        List<StoreFile> files = [];

        if (await ctr.ExistsAsync())
        {
            await foreach (BlobItem blob in ctr.GetBlobsAsync())
                files.Add(FromBlobClient(ctr.GetBlobClient(blob.Name)));
        }

        return files;
    }

    public async Task<StoreFile?> GetFile(string container, string file)
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);

        if (await ctr.ExistsAsync())
        {
            BlobClient blob = ctr.GetBlobClient(file);

            return await blob.ExistsAsync()
                ? FromBlobClient(blob)
                : null;
        }
        else
            return null;
    }

    public async Task<ApiMessage<StoreFile>> Upload(
        IFormFile upload,
        string container
    )
    {
        BlobContainerClient ctr = store.GetBlobContainerClient(container);
        await ctr.CreateIfNotExistsAsync();
        BlobClient blob = ctr.GetBlobClient(upload.FileName);
        await blob.UploadAsync(upload.OpenReadStream(), true);

        return new(
            FromBlobClient(ctr.GetBlobClient(upload.FileName)),
            $"Blob '{upload.FileName}' was uploaded to container '{container}'"
        );
    }

    static StoreFile FromBlobClient(BlobClient blob) => new()
    {
        Container = blob.BlobContainerName,
        Name = blob.Name,
        Uri = blob.Uri,
        Length = blob.GetProperties().Value.ContentLength
    };
}