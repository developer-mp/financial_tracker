//using Npgsql;
//using System.Collections.ObjectModel;

//namespace FinancialTracker.Service
//{
//    public class DataLoadingService
//    {
//        public ObservableCollection<ExpenseItem> LoadData(string connectionString)
//        {
//            ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

//            using (var conn = new NpgsqlConnection(connectionString))
//            {
//                conn.Open();

//                using (var cmd = new NpgsqlCommand("SELECT id, date, expense, category, amount FROM finance", conn))
//                using (var reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        ExpenseItem expense = new ExpenseItem
//                        {
//                            Id = reader.GetString(0),
//                            Date = reader.GetDateTime(1),
//                            Expense = reader.GetString(2),
//                            Category = reader.GetString(3),
//                            Amount = reader.GetDouble(4)
//                        };

//                        expenseList.Add(expense);
//                    }
//                }
//            }

//            return expenseList;
//        }
//    }
//}

using FinancialTracker;
using Npgsql;
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
}

