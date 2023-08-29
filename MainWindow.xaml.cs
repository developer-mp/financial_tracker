using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Npgsql;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private ConfigurationManager _configManager;

        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        private double CalculateTotalExpenses()
        {
            double totalExpenses = 0;
            foreach (ExpenseItem expense in expenseList)
            {
                totalExpenses += expense.Amount;
            }
            return totalExpenses;
        }

        private void UpdateTotalExpensesTextBlock()
        {
            double totalExpenses = CalculateTotalExpenses();
            TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
        }
        public MainWindow()
        {
            InitializeComponent();
            _configManager = new ConfigurationManager();
            TransactionListView.ItemsSource = expenseList;
            LoadData();
            DataContext = this;
            UpdateTotalExpensesTextBlock();
        }
        private void LoadData()
        {
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
        }

        private void TransactionListViewEdit(object sender, MouseButtonEventArgs e)
        {
            if (TransactionListView.SelectedItem != null)
            {
                ExpenseItem selectedExpense = (ExpenseItem)TransactionListView.SelectedItem;

                EditExpenseWindow editExpenseWindow = new EditExpenseWindow(selectedExpense);
                editExpenseWindow.DataUpdated += EditExpenseWindowDataUpdated;
                editExpenseWindow.ShowDialog();
            }
        }

        private void EditExpenseWindowDataUpdated(object sender, EventArgs e)
        {
            expenseList.Clear();
            LoadData();
            UpdateTotalExpensesTextBlock();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
            addExpenseWindow.ShowDialog();
            UpdateTotalExpensesTextBlock();
        }

        private void ReportButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ExpenseItem
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Expense { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
    }
}