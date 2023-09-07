using FinancialTracker.Utils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancialTracker.Service
{
    public class ConfigManager
    {
        private IConfiguration Configuration { get; }

        public ConfigManager()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }
        public QuerySettings GetQuerySettings(string queryName)
        {
            var querySettingsSection = Configuration.GetSection($"Queries:{queryName}");
            return new QuerySettings
            {
                Query = querySettingsSection["Query"]
            };
        }
    }
}
