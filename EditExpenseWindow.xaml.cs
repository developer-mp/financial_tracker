using Npgsql;
using System;
using System.Windows;

namespace FinancialTracker
{
    public partial class EditExpenseWindow : Window
    {
        private ExpenseItem _selectedExpense;

        public EditExpenseWindow(ExpenseItem selectedExpense)
        {
            InitializeComponent();
            _selectedExpense = selectedExpense;

            DatePicker.SelectedDate = _selectedExpense.Date;
            ExpenseTextBox.Text = _selectedExpense.Expense;
            CategoryTextBox.Text = _selectedExpense.Category;
            AmountTextBox.Text = _selectedExpense.Amount.ToString();
        }

        public event EventHandler DataUpdated;

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            _selectedExpense.Date = DatePicker.SelectedDate ?? DateTime.Now;
            _selectedExpense.Expense = ExpenseTextBox.Text;
            _selectedExpense.Category = CategoryTextBox.Text;
            _selectedExpense.Amount = Convert.ToDouble(AmountTextBox.Text);

            string connString = "Host=localhost;Username=admin;Password=admin;Database=finance";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("UPDATE finance SET date = @Date, expense = @Expense, category = @Category, amount = @Amount WHERE id = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("Date", _selectedExpense.Date);
                    cmd.Parameters.AddWithValue("Expense", _selectedExpense.Expense);
                    cmd.Parameters.AddWithValue("Category", _selectedExpense.Category);
                    cmd.Parameters.AddWithValue("Amount", _selectedExpense.Amount);
                    cmd.Parameters.AddWithValue("ID", _selectedExpense.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            // Update the UI and notify the MainWindow if necessary

            DataUpdated?.Invoke(this, EventArgs.Empty);

            Close();
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            string connString = "Host=localhost;Username=admin;Password=admin;Database=finance";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM finance WHERE id = @ID", conn))
                {
                    cmd.Parameters.AddWithValue("ID", _selectedExpense.Id);

                    cmd.ExecuteNonQuery();
                }
            }

            DataUpdated?.Invoke(this, EventArgs.Empty);

            Close();
        }
    }
}
