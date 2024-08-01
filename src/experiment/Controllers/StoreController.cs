using Blob.Api.Store;
using Microsoft.AspNetCore.Mvc;

namespace Blob.Api.Controllers;

[Route("[controller]")]
public class StoreController(IStore store) : ApiController
{
    readonly IStore store = store;

    [HttpGet("[action]/{container}")]
    public async Task<IActionResult> CreateContainer(
        [FromRoute] string container
    ) => ApiResult(await store.CreateContainer(container));

    [HttpGet("[action]/{container}/{file}")]
    public async Task<FileContentResult> Download(
        [FromRoute] string container,
        [FromRoute] string file
    ) => await store.Download(container, file);

    [HttpGet("[action]")]
    public async Task<IActionResult> GetContainers() =>
        ApiResult(await store.GetContainers());

    [HttpGet("[action]/{container}")]
    public async Task<IActionResult> GetContainer(
        [FromRoute] string container
    ) => ApiResult(await store.GetContainer(container));

    [HttpGet("[action]/{container}")]
    public async Task<IActionResult> GetFiles(
        [FromRoute] string container
    ) => ApiResult(await store.GetFiles(container));

    [HttpGet("[action]/{container}/{file}")]
    public async Task<IActionResult> GetFile(
        [FromRoute] string container,
        [FromRoute] string file
    ) => ApiResult(await store.GetFile(container, file));

    [HttpPost("[action]/{container}")]
    public async Task<IActionResult> Upload(
        IFormFile upload,
        [FromRoute] string container
    ) => ApiResult(await store.Upload(upload, container));

    [HttpDelete("[action]/{container}")]
    public async Task<IActionResult> DeleteContainer(
        [FromRoute] string container
    ) => ApiResult(await store.DeleteContainer(container));

    [HttpDelete("[action]/{container}/{file}")]
    public async Task<IActionResult> DeleteFile(
        [FromRoute] string container,
        [FromRoute] string file
    ) => ApiResult(await store.DeleteFile(container, file));
}