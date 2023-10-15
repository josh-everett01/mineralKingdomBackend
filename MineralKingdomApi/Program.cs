using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MineralKingdomApi;
using MineralKingdomApi.Data;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using System;
using System.IO;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


// Add DbContext to the services container.
builder.Services.AddDbContext<MineralKingdomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registering repositories and services for DI

// Mineral
builder.Services.AddScoped<IMineralRepository, MineralRepository>();
builder.Services.AddScoped<IMineralService, MineralService>();

// Auction
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IAuctionService, AuctionService>();

// Auction Status
builder.Services.AddScoped<IAuctionStatusService, AuctionStatusService>();
builder.Services.AddScoped<IAuctionStatusRepository, AuctionStatusRepository>();

// Configure user secrets for the development environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Startup>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
