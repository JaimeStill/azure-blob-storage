# API Integration

Create abstractions around file-based operations with an implementation targeting an Azure Blob Storage account. Expose the implementation as a service to a .NET REST API and expose the methods through a controller.

## Experiment

1. Create `StoreContainer`:

    ```cs
    public record StoreContainer
    {
        public string Name { get; set; } = string.Empty;
    }
    ```

2. Create `StoreFile`:

    ```cs
    public record StoreFile
    {
        public string Container { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Uri? Uri { get; set; }
        public long? Length { get; set; }
    }
    ```

3. Create `IStore`:

    ```cs
    public interface IStore
    {
        /*
            Root specifies teh baseline for file-based operations.
            In a physical filesystem, this would be the root directory.
            In Azure Blob Storage, this would be the storage account.
        */
        string Root { get; }

        Task<ApiMessage<StoreFile>> Delete(string container, string name);
        Task<FileContentResult> Download(string container, string name);
        Task<List<StoreContainer>> GetContainers();
        Task<StoreContainer?> GetContainer(string container);
        Task<List<StoreFile>> GetFiles(string container);
        Task<StoreFile?> GetFile(string container, string file);
        Task<ApiMessage<StoreFile>> Upload(IFormFile upload, string container, string file);
    }
    ```
    
4. Create a class that implements `IStore` and interfaces with Azure Blob Storage.

5. Register the instance of `IStore` as a service and expose the functionality through a controller.

6. Test and verify the functionality.

## Result