using System.Text.Json;
using System.Text.Json.Serialization;
using Blob.Api.Store;
using Microsoft.AspNetCore.Mvc;

namespace Blob.Api.Extensions;
public static class Configuration
{
    public static IMvcBuilder ConfigureApiServices(this WebApplicationBuilder builder) =>
        builder
            .Services
            .AddControllers()
            .AddJsonOptions(HttpJsonOptions);

    public static WebApplicationBuilder ConfigureAzureBlobStore(this WebApplicationBuilder builder)
    {
        string account = builder
            .Configuration
            .GetValue<string>("StorageAccount")
        ?? throw new Exception("Configuration: StorageAccount setting is required");

        builder.Services.AddScoped<IStore, AzureBlobStore>(_ => new AzureBlobStore(account));

        return builder;
    }

    static Action<JsonOptions> HttpJsonOptions => (JsonOptions options) =>
        JsonOptions(options.JsonSerializerOptions);

    static JsonSerializerOptions JsonOptions(JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter());
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        return options;
    }
}