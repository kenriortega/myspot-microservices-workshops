using Micro.Framework;
using Micro.Handlers;
using Micro.Messaging;
using MySpot.Services.Availability.Application;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Infrastructure;

var builder = WebApplication
    .CreateBuilder(args)
    .AddMicroFramework();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", (AppInfo appInfo) => appInfo).WithTags("API").WithName("Info");

app.MapGet("/ping", () => "pong").WithTags("API").WithName("Pong");

app.MapPost("/resources", async (AddResource command, IDispatcher dispatcher) =>
{
    await dispatcher.SendAsync(command);
    return Results.NoContent();
}).WithTags("Resources").WithName("Add resource");

app.Subscribe()
    .Command<AddResource>()
    .Command<DeleteResource>()
    .Command<ReserveResource>()
    .Command<ReleaseResourceReservation>();

app.UseMicroFramework().Run();
