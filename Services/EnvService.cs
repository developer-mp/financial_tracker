using FinancialTracker.Models;
using Microsoft.Extensions.Configuration;
using System;
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

            if (DbConnectionSection == null)
            {
                throw new InvalidOperationException("Db configuration not found");
            }

            var host = DbConnectionSection["Host"];
            var username = DbConnectionSection["Username"];
            var password = DbConnectionSection["Password"];
            var dbName = DbConnectionSection["DbName"];

            if (string.IsNullOrEmpty(host))
            {
                throw new InvalidOperationException("Host not found in Db configuration");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new InvalidOperationException("Username not found in Db configuration");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Password not found in Db configuration");
            }

            if (string.IsNullOrEmpty(dbName))
            {
                throw new InvalidOperationException("DbName not found in Db configuration");
            }

            return new DbConnection
            {
                Host = host,
                Username = username,
                Password = password,
                DbName = dbName
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

            if (pythonDLLPath == null)
            {
                throw new InvalidOperationException("Python DDL path section not found");
            }

            var dllPath = pythonDLLPath["DLLPath"];

            if (dllPath == null)
            {
                throw new InvalidOperationException("Python DLL path not found");
            }

            return dllPath;
        }
    }
}
