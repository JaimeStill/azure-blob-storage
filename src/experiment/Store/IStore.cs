using Blob.Api.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Blob.Api.Store;
public interface IStore
{
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