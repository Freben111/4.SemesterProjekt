using Dapr;
using Dapr.Client;
using ForumService.Application;
using ForumService.Application.Interfaces;
using ForumService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddControllers().AddDapr();
builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationDependencies();
builder.Services.AddInfrastructure(builder.Configuration);





var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
