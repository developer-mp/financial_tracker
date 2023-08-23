using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Npgsql;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        public MainWindow()
        {
            InitializeComponent();

            TransactionListView.ItemsSource = expenseList;

            LoadData();
        }

        private void LoadData()
        {
            string connString = "Host=localhost;Username=admin;Password=admin;Database=finance";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT date, expense, category, amount FROM finance", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExpenseItem expense = new ExpenseItem
                        {
                            Date = reader.GetDateTime(0),
                            Expense = reader.GetString(1),
                            Category = reader.GetString(2),
                            Amount = reader.GetDouble(3)
                        };

                        expenseList.Add(expense);
                    }
                }
            }
        }

        private void TransactionListViewModify(object sender, MouseButtonEventArgs e)
        {
            if (TransactionListView.SelectedItem != null)
            {
                ExpenseItem selectedExpense = (ExpenseItem)TransactionListView.SelectedItem;

                ModifyExpenseWindow modifyExpenseWindow = new ModifyExpenseWindow(selectedExpense);
                modifyExpenseWindow.ShowDialog();

                // After editing or deleting, you might want to update the list or UI here
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            // Create a new expense item
            ExpenseItem newExpense = new ExpenseItem
            {
                Date = DateTime.Now,
                Expense = "Expense",
                Category = "Category",
                Amount = 100.00,
            };

            // Add the expense item to the collection
            expenseList.Add(newExpense);
        }

        private void ReportButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ExpenseItem
    {
        public DateTime Date { get; set; }
        public string Expense { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
    }
}