using Api;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPresentationServices()
                .AddApplicationServices()
                .AddInfrastructureServices();

var app = builder.Build();

app.UsePresentationServices();

app.Run();
