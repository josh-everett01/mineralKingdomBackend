using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MineralKingdomApi.Data
{
    public class MineralKingdomContextFactory : IDesignTimeDbContextFactory<MineralKingdomContext>
    {
        public MineralKingdomContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<MineralKingdomContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new MineralKingdomContext(optionsBuilder.Options, configuration);
        }
    }
}
