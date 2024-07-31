namespace Blob.Api.Store;
public record StoreFile
{
    public string Container { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Uri? Uri { get; set; }
    public long? Length { get; set; }
}