using FinancialTracker;
using Npgsql;
using System;
using System.Collections.ObjectModel;

public class DataLoadingService
{
    public ObservableCollection<ExpenseItem> LoadData(string connectionString, QuerySettings querySettings)
    {
        ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand(querySettings.Query, conn))
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
        }

        return expenseList;
    }

    public double ExecuteScalarQuery(string connectionString, QuerySettings querySettings)
    {
        using (var conn = new NpgsqlConnection(connectionString))
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand(querySettings.Query, conn))
            {
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToDouble(result);
                }
            }
        }

        return 0;
    }
}

