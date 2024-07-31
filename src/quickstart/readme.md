# Azure Blob Storage - Quickstart

The sections below will describe the prerequisites and setup required to execute the quickstart project.

## Prerequisites

* Download and install the [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli).

* Download and install the latest [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

* An [Azure subscription](https://azure.microsoft.com/free/)

* An [Azure storage account](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create)

## Setup

The steps below describe how to setup the local development environment to allow the identity logged into Azure CLI to be associated with `DefaultAzureCredential`.

1. Login to Azure:

    ```bash
    az login
    ```

2. Capture the **User principal name** of the account you use to login to the Azure CLI from the [Microsoft Entra Users](https://portal.azure.com/#view/Microsoft_AAD_UsersAndTenants/UserManagementMenuBlade/~/AllUsers) interface.

3. Capture the ID of the Azure storage account:

    ```pwsh
    az storage account show `
        --resource-group '<resource-group>' `
        --name '<storage-account-name>' `
        --query id
    ```

4. Using the **user principal name** for your azure account as well as the output **id** from the previous command, assign the *Storage Blob Data Contributor* role to your storage account:

    ```pwsh
    az role assignment create `
        --assignee "<user-principal-name>" `
        --role "Storage Blob Data Contributor" `
        --scope "<storage-account-id>"
    ```

In most cases it will take a minute or two for the role assignment to propogate in Azure, but in rare cases it may take up to eight minutes. If you receive authentication errors when you first run the CLI, wait a few moments and try again.

## Run

To test and run the [**quickstart**](./CommandBuilder.cs#L27), simply execute the following, replacing `<storage-account>` with the name of your storage account:

```pwsh
dotnet run -- -a '<storage-account>'
```

You should see the following output:

```
Connecting service client to storage account <storage-account>

Initializing container 'templates'
Creating container 'templates'
Container 'templates' created

Listing containers in '<storage-account>'
        templates

Uploading file 'ss-5.pdf' to 'templates'
'ss-5.pdf' uploaded to 'https://<storage-account>.blob.core.windows.net/templates/ss-5.pdf'

Listing blobs in 'templates'
        ss-5.pdf

Downloading 'ss-5.pdf' to 'C:\azure-blob-storage\src\quickstart\bin\Debug\net8.0\templates\ss-5-download.pdf'

Press any key to begin cleanup: 

Deleting container 'templates'
Deleting downloaded files
Done
```

If you inspect the storage container `templates` before beginning cleanup, you'll see that the `ss-5.pdf` file was uploaded:

![image](https://github.com/user-attachments/assets/c9d790c6-d7c6-41fb-b7fd-f1bc015b8bc3)

Additionally, if you inspect the [**bin/Debug/net8.0/templates**](./bin/Debug/net8.0/templates/) directory, you will find `ss-5-download.pdf` has been downloaded using `BlobClient`:

![image](https://github.com/user-attachments/assets/6c7a8120-372b-4a22-811d-6f812cfeec26)