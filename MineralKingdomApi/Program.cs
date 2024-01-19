using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MineralKingdomApi.Data;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using Stripe;
using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;


DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mineral Kingdom API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Configure Swagger to use JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Add DbContext to the services container.
builder.Services.AddDbContext<MineralKingdomContext>(options =>
    options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")));

// Configure Authentication and Authorization
var secretKey = builder.Configuration.GetValue<string>("SECRET_KEY");
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT Secret Key is missing");
}
var key = Encoding.ASCII.GetBytes(secretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(nameof(UserRole.Admin)));
});

// Registering repositories and services for Dependency Injection
builder.Services.AddScoped<IJwtService, JwtService>();

// User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Mineral
builder.Services.AddScoped<IMineralRepository, MineralRepository>();
builder.Services.AddScoped<IMineralService, MineralService>();

// Auction
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IAuctionService, AuctionService>();

// Auction Status
builder.Services.AddScoped<IAuctionStatusService, AuctionStatusService>();
builder.Services.AddScoped<IAuctionStatusRepository, AuctionStatusRepository>();

// Auction notification service
builder.Services.AddHostedService<AuctionNotificationService>();

// Bids
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IBidService, BidService>();

// Shopping Cart
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

// Payment
builder.Services.AddScoped<IPaymentDetailsRepository, PaymentDetailsRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Correspondence
builder.Services.AddScoped<ICorrespondenceRepository, CorrespondenceRepository>();
builder.Services.AddScoped<ICorrespondenceService, CorrespondenceService>();

// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowVueApp", builder =>
//    {
//        builder.WithOrigins("https://localhost:8080")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//    });
//});

// Configure CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowVueApp", builder =>
//    {
//        builder.WithOrigins("https://mineralkingdomfrontend.onrender.com")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//    });
//});


// Configure user secrets for the development environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    StripeConfiguration.ApiKey = builder.Configuration["STRIPE_API_KEY"];
}

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    // Configure Kestrel to use the HTTPS certificate
//    serverOptions.Listen(IPAddress.Any, 443, listenOptions =>
//    {
//        listenOptions.UseHttps("/https/certificate.pfx", ""); // Path to the certificate in the container and the password (empty if no password)
//    });
//});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Listen on port 10000 for any IP address (0.0.0.0)
    serverOptions.Listen(IPAddress.Any, 10000);
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply global CORS policy
app.UseCors(builder => builder
    .WithOrigins("https://mineralkingdomfrontend.onrender.com") // Frontend's URL
    .AllowAnyHeader() // Allows any header
    .AllowAnyMethod() // Allows any HTTP method
    .AllowCredentials()); // Allows credentials (such as cookies, authorization headers, etc.)

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
