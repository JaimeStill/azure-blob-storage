using Blob.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApiServices();
builder.ConfigureAzureBlobStore();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.MapControllers();

app.Run();