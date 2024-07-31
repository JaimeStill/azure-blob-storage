namespace Blob.Api.Messages;
public class ApiMessage<T> : IApiMessage
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Error { get; set; }
    public bool HasData => Data is not null;

    public ApiMessage() { }

    public ApiMessage(string action, Exception ex)
    {
        Error = true;
        Message = $"{typeof(T).Name}.{action} failed: {ex.Message}";
    }

    public ApiMessage(string action, string error)
    {
        Error = true;
        Message = $"{typeof(T).Name}.{action} failed: {error}";
    }

    public ApiMessage(T data, string message = "Operation completed successfully")
    {
        Data = data;
        Message = message;
    }

    public ApiMessage(ValidationMessage validation)
    {
        Error = true;
        Message = validation.Message;
    }
}