using Aurabox.Server.Service;
using Aurabox.Server.Workers;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddLogging(lb => lb.ClearProviders().AddSerilog());
builder.Services.AddSingleton<AuraboxContextService>();

builder.Services.AddHostedService<ServerWorker>();

var app = builder.Build();

await app.RunAsync();