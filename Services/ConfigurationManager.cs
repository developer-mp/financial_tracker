using FinancialTracker.Utils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancialTracker.Service
{
    public class ConfigurationManager
    {
        private IConfiguration Configuration { get; }

        public ConfigurationManager()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .Build();
        }

        public DbSettings GetDbSettings()
        {
            var dbSettingsSection = Configuration.GetSection("Db");
            return new DbSettings
            {
                Host = dbSettingsSection["Host"],
                Username = dbSettingsSection["Username"],
                Password = dbSettingsSection["Password"],
                DbName = dbSettingsSection["DbName"]
            };
        }

        public string GetConnectionString()
        {
            var dbSettings = GetDbSettings();
            return $"Host={dbSettings.Host};Username={dbSettings.Username};Password={dbSettings.Password};Database={dbSettings.DbName}";
        }

        public QuerySettings GetQuerySettings(string queryName)
        {
            var querySettingsSection = Configuration.GetSection($"Queries:{queryName}");
            return new QuerySettings
            {
                Query = querySettingsSection["Query"]
            };
        }

        public string GetPythonDLLPath()
        {
            var pythonDLLPath = Configuration.GetSection("Python");
            return pythonDLLPath["DLLPath"];
        }
    }
}
