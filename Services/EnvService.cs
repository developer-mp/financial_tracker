using FinancialTracker.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinancialTracker.Service
{
    public class EnvService
    {
        private IConfiguration Configuration { get; }

        public EnvService()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("env.json")
                .Build();
        }

        public DbConnection GetDbConnection()
        {
            var DbConnectionSection = Configuration.GetSection("Db");
            return new DbConnection
            {
                Host = DbConnectionSection["Host"],
                Username = DbConnectionSection["Username"],
                Password = DbConnectionSection["Password"],
                DbName = DbConnectionSection["DbName"]
            };
        }

        public string GetConnectionString()
        {
            var DbConnection = GetDbConnection();
            return $"Host={DbConnection.Host};Username={DbConnection.Username};Password={DbConnection.Password};Database={DbConnection.DbName}";
        }

        public string GetPythonDLLPath()
        {
            var pythonDLLPath = Configuration.GetSection("Python");
            return pythonDLLPath["DLLPath"];
        }
    }
}
