using Npgsql;
using System.Collections.ObjectModel;

namespace FinancialTracker
{
    public class DataLoader
    {
        private ConfigurationManager _configManager;

        public DataLoader(ConfigurationManager configManager)
        {
            _configManager = configManager;
        }

        public ObservableCollection<ExpenseItem> LoadData()
        {
            ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

            string connString = _configManager.GetConnectionString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT id, date, expense, category, amount FROM finance", conn))
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
}
