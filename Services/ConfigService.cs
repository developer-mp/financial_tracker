using FinancialTracker.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FinancialTracker.Service
{
    public class ConfigService
    {
        private IConfiguration Configuration { get; }

        public ConfigService()
        {
            try
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json")
                    .Build();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error configuring service: {ex.Message}");
            }

        }
        public DbQuery GetDbQuery(string queryName)
        {
            try
            {
                var DbQuerySection = Configuration.GetSection($"Queries:{queryName}");
                return new DbQuery
                {
                    Query = DbQuerySection["Query"]
                };
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error getting DB query: {ex.Message}");
                return null;
            }
        }

        public List<string> GetCategoryColors()
        {
            try
            {
                var Category = Configuration.GetSection("Categories").Get<List<Category>>();
                return Category.Select(category => category.CategoryColor).ToList();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error getting category colors: {ex.Message}");
                return null;
            }
        }

        public List<string> GetCategoryNames()
        {
            try
            {
                var Category = Configuration.GetSection("Categories").Get<List<Category>>();
                return Category.Select(category => category.CategoryName).ToList();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error getting category names: {ex.Message}");
                return null;
            }
        }

        public DefaultSorting GetDefaultSorting()
        {
            try
            {
                var defaultSortingSection = Configuration.GetSection("DefaultSorting");
                return new DefaultSorting
                {
                    SortColumn = defaultSortingSection["SortColumn"],
                    SortDirection = defaultSortingSection["SortDirection"].ToLower() == "ascending" ? "Ascending" : "Descending"
                };
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error default sorting service: {ex.Message}");
                return null;
            }
        }

        public ErrorMessage GetErrorMessage(string errorName, bool isSuccess = false)
        {
            try
            {
                var errorMessageSection = Configuration.GetSection($"Errors:{errorName}");

                return new ErrorMessage
                {
                    Success = isSuccess ? errorMessageSection["Success"] : errorMessageSection["Error"],
                    Error = errorMessageSection["Error"]
                };
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error getting error message: {ex.Message}");
                return null;
            }
        }
    }
}
