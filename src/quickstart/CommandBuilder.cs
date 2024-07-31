using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Blob.Cli;
public static class CommandBuilder
{
    public static RootCommand Create()
    {
        RootCommand root = new("Azure Blob CLI")
        {
            Handler = CommandHandler.Create(Execution)
        };

        root.AddOption(
            new Option<string>(
                ["--account", "-a"],
                description: "Azure Blob Storage account name"
            )
        );

        return root;
    }

    static Func<string, Task> Execution => async (account) =>
    {
        string accountUri = $"https://{account}.blob.core.windows.net";

        string root = AppDomain.CurrentDomain.BaseDirectory;
        const string directory = "templates";
        const string template = "ss-5.pdf";
        const string download = "ss-5-download.pdf";

        FileInfo file = new(
            Path.Combine(root, directory, template)
        );

        if (file.Exists)
        {
            /*
                Initialize BlobServiceClient
            */
            Console.WriteLine($"Connecting service client to storage account '{account}'");
            BlobServiceClient svc = new(
                new Uri(accountUri),
                new DefaultAzureCredential()
            );

            Console.WriteLine();

            /*
                Initialize BlobContainerClient
            */
            Console.WriteLine($"Initializing container '{directory}'");
            BlobContainerClient container = svc.GetBlobContainerClient(directory);

            if (container.Exists())
                Console.WriteLine($"Connected to '{container.Name}'");
            else
            {
                Console.WriteLine($"Creating container '{directory}'");
                await container.CreateAsync();
                Console.WriteLine($"Container '{container.Name}' created");
            }

            Console.WriteLine();

            /*
                List containers in storage account
            */
            Console.WriteLine($"Listing containers in '{account}'");
            await foreach (BlobContainerItem c in svc.GetBlobContainersAsync())
                Console.WriteLine($"\t{c.Name}");

            Console.WriteLine();

            /*
                Initialize BlobClient and upload a file
            */
            Console.WriteLine($"Uploading file '{file.Name}' to '{directory}'");
            BlobClient blob = container.GetBlobClient(file.Name);
            await blob.UploadAsync(file.FullName, true);
            Console.WriteLine($"'{file.Name}' uploaded to '{blob.Uri}'");
            Console.WriteLine();

            /*
                List blobs in container
            */
            Console.WriteLine($"Listing blobs in '{directory}'");
            await foreach (BlobItem b in container.GetBlobsAsync())
                Console.WriteLine($"\t{b.Name}");

            Console.WriteLine();

            /*
                Download a blob
            */
            string downloadPath = Path.Combine(root, directory, download);
            Console.WriteLine($"Downloading '{template}' to '{downloadPath}'");
            await blob.DownloadToAsync(downloadPath);
            Console.WriteLine();

            /*
                Delete a container
            */
            Console.Write("Press any key to begin cleanup: ");
            Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine($"Deleting container '{directory}'");
            await container.DeleteAsync();

            Console.WriteLine("Deleting downloaded files");
            FileInfo downloadFile = new(downloadPath);
            if (downloadFile.Exists)
                downloadFile.Delete();

            Console.WriteLine("Done");
        }

    };
}