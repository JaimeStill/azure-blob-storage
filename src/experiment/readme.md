# API Integration

Create abstractions around file-based operations with an implementation targeting an Azure Blob Storage account. Expose the implementation as a service to a .NET REST API and expose the methods through a controller.

To run the experiment:

1. Follow the instructions in the [**Prerequisites**](../quickstart/readme.md#prerequisites) and [**Setup**](../quickstart/readme.md#setup) sections of the quickstart readme.

2. In the terminal, navigate to this directory and execute `dotnet run`.

3. Access the API interface at http://localhost:5002/swagger

## Experiment

1. Create the following infrastructure to support abstracting a backing store for unstructured data:

    ```cs
    public record StoreContainer
    {
        public string Name { get; set; } = string.Empty;
    }
    ```

    ```cs
    public record StoreFile
    {
        public string Container { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Uri? Uri { get; set; }
        public long? Length { get; set; }
    }
    ```

    ```cs
    public interface IStore
    {
        /*
            Root specifies teh baseline for file-based operations.
            In a physical filesystem, this would be the root directory.
            In Azure Blob Storage, this would be the storage account.
        */
        string Root { get; }

        Task<ApiMessage<StoreContainer>> CreateContainer(string container);
        Task<FileContentResult> Download(string container, string name);
        Task<List<StoreContainer>> GetContainers();
        Task<StoreContainer?> GetContainer(string container);
        Task<List<StoreFile>> GetFiles(string container);
        Task<StoreFile?> GetFile(string container, string file);
        Task<ApiMessage<StoreFile>> Upload(IFormFile upload, string container);
        Task<ApiMessage<StoreContainer>> DeleteContainer(string container);
        Task<ApiMessage<StoreFile>> DeleteFile(string container, string name);
    }
    ```
    
2. Create a class that implements `IStore` and interfaces with Azure Blob Storage.

3. Register the instance of `IStore` as a service and expose the functionality through a controller.

4. Test and verify the functionality.

## Result

The [**`AzureBlobStore`**](./Store/AzureBlobStore.cs) class implements the `IStore` interface as specified in the experiment synopsis. The following infrastructure facilitates registering `AzureBlobStore` as as service of type `IStore`:

* [**`appsettings.Development.json`**](./appsettings.Development.json) - specifies the Azure Blob Storage account used as the basis for the store.

* [**`Configuration.ConfigureAzureBlobStore`**](./Extensions/Configuration.cs#L15) - Retrieves the `StorageAccount` value from configuration and registers an `IStore` service with `AzureBlobStore` as the implementation. The factory pattern is used to tell the depenency injection container to initialize the instance of `AzureBlobStore` using the retrieved `StorageAccount` value as the constructor parameter.

* [**`Program`**](./Program.cs#L6) - Calls `ConfigureAzureBlobStore`, defined above.

The service is then injected into [**`StoreController`**](./Controllers/StoreController.cs), which exposes the public methods of `AzureBlobStore` as HTTP endpoints.

### Verification

1. Create a Container:

    ![image](https://github.com/user-attachments/assets/ba8c3930-89cd-4d4a-a0b3-c65f55b646b1)

    ![image](https://github.com/user-attachments/assets/32178d2f-e135-4326-bfec-944fbbc79385)

2. Get Containers:

    ![image](https://github.com/user-attachments/assets/9b050bb3-d4a2-40ab-a0b7-448238ebad7c)

3. Get Container:

    ![image](https://github.com/user-attachments/assets/300cecc6-8f53-46d1-a35a-fe3f82aa4ede)

4. Upload:

    ![image](https://github.com/user-attachments/assets/22f8e8dd-86c4-440e-bf2d-14f4d068a713)

    ![image](https://github.com/user-attachments/assets/bfea7c44-6de4-4160-9eea-60be8a44cd0c)

5. Get Files:

    ![image](https://github.com/user-attachments/assets/93a402ad-4133-454f-831c-0abe5ed35bc2)

6. Get File:

    ![image](https://github.com/user-attachments/assets/c389b8fd-b31b-40a8-8e67-db5205b8628b)

7. Download:

    ![image](https://github.com/user-attachments/assets/23daeb44-5b85-4b2b-b116-c0183ce9f80b)

    ![image](https://github.com/user-attachments/assets/f759c545-9fe5-4b6e-90b4-3f79ef16c9c6)

8. Delete File:

    ![image](https://github.com/user-attachments/assets/9a2415a1-e31a-4b40-b0de-30f925d1b5e3)

    ![image](https://github.com/user-attachments/assets/cf296d10-511d-4f89-9839-3c44b8c42ef3)

9. Delete Container:

    ![image](https://github.com/user-attachments/assets/4acd89e6-0cd9-4ce6-b004-f1456692c33f)

    ![image](https://github.com/user-attachments/assets/83cf0aa1-68cd-4f6b-acf5-1221733212da)