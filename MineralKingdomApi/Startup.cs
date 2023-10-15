using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MineralKingdomApi.Data;
using MineralKingdomApi.Controllers;
using Microsoft.OpenApi.Models;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using System.Reflection;
using System.IO;

namespace MineralKingdomApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configure DbContext with SQL Server provider
            services.AddDbContext<MineralKingdomApi.Data.MineralKingdomContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });


            // Registering repositories and services for DI
/*
            // Mineral
            services.AddScoped<IMineralRepository, MineralRepository>();
            services.AddScoped<IMineralService, MineralService>();

            // Auction
            services.AddScoped<IAuctionService, AuctionService>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();

            // Auction Status
            services.AddScoped<IAuctionStatusService, AuctionStatusService>();
            services.AddScoped<IAuctionStatusRepository, AuctionStatusRepository>();

            */
            // Add other services, controllers, etc. here as needed
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5113")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mineral Kingdom API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Configure middleware and routing here

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MineralKingdom API V1");
            });
        }
    }
}
