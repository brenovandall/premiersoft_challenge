using Microsoft.Extensions.Configuration;

namespace Infrastructure.Extensions
{
    internal static class ConnectionStringBuilder
    {
        public static string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            return config.GetConnectionString("DbConnection")!;
        }
    }
}
