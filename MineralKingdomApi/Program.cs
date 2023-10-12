using Microsoft.EntityFrameworkCore;
using MineralKingdomApi;
using MineralKingdomApi.Data;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext to the services container.
builder.Services.AddDbContext<MineralKingdomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registering repositories and services for DI
builder.Services.AddScoped<IMineralRepository, MineralRepository>();
builder.Services.AddScoped<IMineralService, MineralService>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IAuctionService, AuctionService>();

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
