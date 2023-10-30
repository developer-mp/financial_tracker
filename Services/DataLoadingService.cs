using FinancialTracker.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FinancialTracker.DataLoading
{

    public class DataLoadingService
    {
        private static void ExecuteDbCommand(string connectionString, Action<NpgsqlConnection, NpgsqlCommand> commandAction)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            commandAction(conn, cmd);
        }
        public static ObservableCollection<ExpenseItem> LoadExpenses(string connectionString, DbQuery DbQuery, DateTime startDate, DateTime endDate, string category, double minAmount, double maxAmount)
        {
            ObservableCollection<ExpenseItem> expenseList = new();

            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;

                cmd.Parameters.AddWithValue("StartDate", startDate);
                cmd.Parameters.AddWithValue("EndDate", endDate);
                cmd.Parameters.AddWithValue("Category", category);
                cmd.Parameters.AddWithValue("MinAmount", minAmount);
                cmd.Parameters.AddWithValue("MaxAmount", maxAmount);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExpenseItem expense = new()
                        {
                            Id = reader.GetString(0),
                            Date = reader.GetDateTime(1),
                            Expense = reader.GetString(2),
                            Category = reader.GetString(3),
                            Amount = reader.GetDouble(4)
                        };

                        expenseList.Add(expense);
                    }
                }
            });

            return expenseList;
        }

        public static double LoadTotalExpenses(string connectionString, DbQuery DbQuery, DateTime startDate, DateTime endDate)
        {
            double totalExpenses = 0;

            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;
                cmd.Parameters.AddWithValue("StartDate", startDate);
                cmd.Parameters.AddWithValue("EndDate", endDate);

                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    totalExpenses = Convert.ToDouble(result);
                }
            });

            return totalExpenses;
        }

        public static List<ExpenseByCategory> LoadExpensesByCategory(string connectionString, DbQuery DbQuery, DateTime startDate, DateTime endDate)
        {
            List<ExpenseByCategory> expensesByCategory = new();

            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;
                cmd.Parameters.AddWithValue("StartDate", startDate);
                cmd.Parameters.AddWithValue("EndDate", endDate);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExpenseByCategory expense = new ExpenseByCategory
                        {
                            Category = reader.GetString(0),
                            TotalAmount = reader.GetDouble(1)
                        };

                        expensesByCategory.Add(expense);
                    }
                }
            });

            return expensesByCategory;
        }

        public static void AddExpense(string connectionString, DbQuery DbQuery, ExpenseItem newExpense)
        {
            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;

                cmd.Parameters.AddWithValue("Id", newExpense.Id);
                cmd.Parameters.AddWithValue("Date", newExpense.Date);
                cmd.Parameters.AddWithValue("Expense", newExpense.Expense);
                cmd.Parameters.AddWithValue("Category", newExpense.Category);
                cmd.Parameters.AddWithValue("Amount", newExpense.Amount);

                cmd.ExecuteNonQuery();
            });
        }

        public static void UpdateExpense(string connectionString, DbQuery DbQuery, ExpenseItem updatedExpense)
        {
            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;

                cmd.Parameters.AddWithValue("Id", updatedExpense.Id);
                cmd.Parameters.AddWithValue("Date", updatedExpense.Date);
                cmd.Parameters.AddWithValue("Expense", updatedExpense.Expense);
                cmd.Parameters.AddWithValue("Category", updatedExpense.Category);
                cmd.Parameters.AddWithValue("Amount", updatedExpense.Amount);

                cmd.ExecuteNonQuery();
            });
        }

        public static void DeleteExpense(string connectionString, DbQuery DbQuery, ExpenseItem selectedExpense)
        {
            ExecuteDbCommand(connectionString, (conn, cmd) =>
            {
                cmd.CommandText = DbQuery.Query;
                cmd.Parameters.AddWithValue("Id", selectedExpense.Id);
                cmd.ExecuteNonQuery();
            });
        }
    }
}
