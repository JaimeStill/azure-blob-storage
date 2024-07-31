using System.CommandLine;
using Blob.Cli;

await CommandBuilder
    .Create()
    .InvokeAsync(args);