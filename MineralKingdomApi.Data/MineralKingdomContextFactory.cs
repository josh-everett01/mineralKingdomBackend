using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using DotNetEnv;
using System.IO;

namespace MineralKingdomApi.Data
{
    public class MineralKingdomContextFactory : IDesignTimeDbContextFactory<MineralKingdomContext>
    {
        public MineralKingdomContext CreateDbContext(string[] args)
        {
            // Load .env variables
            Env.Load();

            // Retrieve the connection string from environment variables
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            // Ensure the connection string is available
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string was not found in the environment variables.");
            };

            var optionsBuilder = new DbContextOptionsBuilder<MineralKingdomContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MineralKingdomContext(optionsBuilder.Options);
        }
    }
}
