using FinancialTracker.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class DataLoadingService
{
    private void ExecuteDbCommand(string connectionString, Action<NpgsqlConnection, NpgsqlCommand> commandAction)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                commandAction(conn, cmd);
            }
        }
    }
    public ObservableCollection<ExpenseItem> LoadExpenses(string connectionString, DbQuery DbQuery)
    {
        ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = DbQuery.Query;
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ExpenseItem expense = new ExpenseItem
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

    public double LoadTotalExpenses(string connectionString, DbQuery DbQuery)
    {
        double totalExpenses = 0;

        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = DbQuery.Query;
            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                totalExpenses = Convert.ToDouble(result);
            }
        });

        return totalExpenses;
    }

    public List<ExpenseByCategory> LoadExpensesByCategory(string connectionString, DbQuery DbQuery)
    {
        List<ExpenseByCategory> expensesByCategory = new List<ExpenseByCategory>();

        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = DbQuery.Query;
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

    public void AddExpense(string connectionString, DbQuery DbQuery, ExpenseItem newExpense)
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

    public void UpdateExpense(string connectionString, DbQuery DbQuery, ExpenseItem updatedExpense)
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

    public void DeleteExpense(string connectionString, DbQuery DbQuery, ExpenseItem selectedExpense)
    {
        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = DbQuery.Query;

            cmd.Parameters.AddWithValue("Id", selectedExpense.Id);

            cmd.ExecuteNonQuery();
        });
    }
}
