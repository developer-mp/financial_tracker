using Npgsql;
using System;
using System.Windows;

namespace FinancialTracker
{
    public partial class AddExpenseWindow : Window
    {
        private MainWindow _mainWindow;

        public AddExpenseWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;

            // Set up the UI elements for editing the selectedExpense
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            ExpenseItem newExpense = new ExpenseItem
            {
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Expense = ExpenseTextBox.Text,
                Category = CategoryTextBox.Text,
                Amount = Convert.ToDouble(AmountTextBox.Text)
            };

            // Save the new expense to the database
            string connString = "Host=localhost;Username=admin;Password=admin;Database=finance";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("INSERT INTO finance (date, expense, category, amount) VALUES (@Date, @Expense, @Category, @Amount)", conn))
                {
                    cmd.Parameters.AddWithValue("Date", newExpense.Date);
                    cmd.Parameters.AddWithValue("Expense", newExpense.Expense);
                    cmd.Parameters.AddWithValue("Category", newExpense.Category);
                    cmd.Parameters.AddWithValue("Amount", newExpense.Amount);

                    cmd.ExecuteNonQuery();
                }
            }

            _mainWindow.expenseList.Add(newExpense);

            Close();
        }
    }
}
