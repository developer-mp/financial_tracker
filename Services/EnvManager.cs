﻿using FinancialTracker.Utils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancialTracker.Service
{
    public class EnvManager
    {
        private IConfiguration Configuration { get; }

        public EnvManager()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("env.json")
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

        public string GetPythonDLLPath()
        {
            var pythonDLLPath = Configuration.GetSection("Python");
            return pythonDLLPath["DLLPath"];
        }
    }
}