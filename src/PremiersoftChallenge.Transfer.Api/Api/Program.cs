using Api;
using Application;
using Infrastructure;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentationServices()
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UsePresentationServices();

await InitialiseSqliteDatabase
    .CreateTablesAsync(builder.Configuration.GetConnectionString("DbConnection")!);

app.Run();
