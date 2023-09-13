using FinancialTracker.Models;
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
        public DbQuery GetDbQuery(string queryName)
        {
            var DbQuerySection = Configuration.GetSection($"Queries:{queryName}");
            return new DbQuery
            {
                Query = DbQuerySection["Query"]
            };
        }

        public List<string> GetCategoryColors()
        {
            var Category = Configuration.GetSection("Categories").Get<List<Category>>();
            return Category.Select(category => category.CategoryColor).ToList();
        }

        public List<string> GetCategoryNames()
        {
            var Category = Configuration.GetSection("Categories").Get<List<Category>>();
            return Category.Select(category => category.CategoryName).ToList();
        }

        public DefaultSorting GetDefaultSorting()
        {
            var defaultSortingSection = Configuration.GetSection("DefaultSorting");
            return new DefaultSorting
            {
                SortColumn = defaultSortingSection["SortColumn"],
                SortDirection = defaultSortingSection["SortDirection"].ToLower() == "ascending" ? "Ascending" : "Descending"
            };
        }

        public ErrorMessage GetErrorMessage(string errorName, bool isSuccess = false)
        {
            var errorMessageSection = Configuration.GetSection($"Errors:{errorName}");

            return new ErrorMessage
            {
                Success = isSuccess ? errorMessageSection["Success"] : errorMessageSection["Error"],
                Error = errorMessageSection["Error"]
            };
        }
    }
}
