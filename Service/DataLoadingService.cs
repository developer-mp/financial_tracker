using FinancialTracker;
using Npgsql;
using System;
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
    public ObservableCollection<ExpenseItem> LoadData(string connectionString, QuerySettings querySettings)
    {
        ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = querySettings.Query;
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

    public double LoadTotalExpenses(string connectionString, QuerySettings querySettings)
    {
        double totalExpenses = 0;

        ExecuteDbCommand(connectionString, (conn, cmd) =>
        {
            cmd.CommandText = querySettings.Query;
            var result = cmd.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                totalExpenses = Convert.ToDouble(result);
            }
        });

        return totalExpenses;
    }
}
