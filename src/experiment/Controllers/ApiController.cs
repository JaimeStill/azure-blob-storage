using Microsoft.AspNetCore.Mvc;
using Blob.Api.Messages;

namespace Blob.Api.Controllers;
public abstract class ApiController : ControllerBase
{
    protected IActionResult ApiResult<T>(T? data) => data switch
    {
        IApiMessage result => HandleApiResult(result),
        ValidationMessage validation => HandleValidation(validation),
        _ => HandleResult(data)
    };

    IActionResult HandleApiResult(IApiMessage result) =>
        result.Error
            ? BadRequest(result.Message)
            : result.HasData
                ? Ok(result)
                : NotFound(result);

    IActionResult HandleValidation(ValidationMessage validation) =>
        validation.IsValid
            ? Ok(validation)
            : BadRequest(validation.Message);

    IActionResult HandleResult<T>(T? result) =>
        result is null
            ? NotFound(result)
            : Ok(result);
}