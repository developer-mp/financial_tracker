using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FinancialTracker
{
    public class ConfigurationManager : IDisposable
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

        public void Dispose()
        {
            // Add any cleanup code here
        }
    }
}
