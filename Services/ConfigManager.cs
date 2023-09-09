using FinancialTracker.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public List<string> GetCategoryColors()
        {
            var categorySettings = Configuration.GetSection("Categories").Get<List<CategorySettings>>();
            return categorySettings.Select(category => category.CategoryColor).ToList();
        }

        public List<string> GetCategoryNames()
        {
            var categorySettings = Configuration.GetSection("Categories").Get<List<CategorySettings>>();
            return categorySettings.Select(category => category.CategoryName).ToList();
        }

    }
}
