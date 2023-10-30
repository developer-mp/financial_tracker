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
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("config.json")
                    .Build();
        }
        public DbQuery GetDbQuery(string queryName)
        {
            var DbQuerySection = Configuration.GetSection($"Queries:{queryName}");
            var queryValue = DbQuerySection?["Query"];

            if (queryValue != null)
            {
                return new DbQuery
                {
                    Query = queryValue
                };
            }
            throw new InvalidOperationException("Query not found in configuration");
        }

        public List<string> GetCategoryColors()
        {
                var Category = Configuration.GetSection("Categories").Get<List<Category>>();

            if (Category == null)
            {
                return new List<string>();
            }

            return Category.Select(category => category.CategoryColor).ToList();
        }

        public List<string> GetCategoryNames()
        {
                var Category = Configuration.GetSection("Categories").Get<List<Category>>();

            if (Category == null)
            {
                return new List<string>();
            }

            return Category.Select(category => category.CategoryName).ToList();
        }

        public DefaultSorting GetDefaultSorting()
        {
            var defaultSortingSection = Configuration.GetSection("DefaultSorting");

            if (defaultSortingSection == null)
            {
                return new DefaultSorting();
            }

            var sortColumn = defaultSortingSection["SortColumn"];
            var sortDirection = defaultSortingSection["SortDirection"]?.ToLower();

            sortColumn ??= "Date";

            return new DefaultSorting
            {
                SortColumn = sortColumn,
                SortDirection = string.Equals(sortDirection, "ascending", StringComparison.OrdinalIgnoreCase) ? "Ascending" : "Descending"
            };
        }

        public ErrorMessage GetErrorMessage(string errorName, bool isSuccess = false)
        {
            var errorMessageSection = Configuration.GetSection($"Errors:{errorName}");

            if (errorMessageSection == null)
            {
                return new ErrorMessage();
            }

            string defaultSuccessMessage = "Operation performed successfully";
            string defaultErrorMessage = "Error performing operation";

            return new ErrorMessage
                {
                Success = isSuccess ? errorMessageSection["Success"] ?? defaultSuccessMessage : errorMessageSection["Error"] ?? "DefaultErrorMessage",
                Error = errorMessageSection["Error"] ?? defaultErrorMessage
            };
        }
    }
}
