using FinancialTracker.Service;
using FinancialTracker.Utils;
using Npgsql;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class EditExpenseWindow : Window
    {
        private ExpenseItem _selectedExpense;
        private EnvManager _configManager;

        public EditExpenseWindow(ExpenseItem selectedExpense)
        {
            InitializeComponent();
            _configManager = new EnvManager();
            _selectedExpense = selectedExpense;

            DatePicker.SelectedDate = _selectedExpense.Date;
            ExpenseTextBox.Text = _selectedExpense.Expense;
            CategoryTextBox.Text = _selectedExpense.Category;
            AmountTextBox.Text = _selectedExpense.Amount.ToString();
        }

        public event EventHandler DataUpdated;

        private async void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedExpense.Date = DatePicker.SelectedDate ?? DateTime.Now;
                _selectedExpense.Expense = ExpenseTextBox.Text;
                _selectedExpense.Category = CategoryTextBox.Text;

                if (!double.TryParse(AmountTextBox.Text, out double amount))
                {
                    MessageBox.Show("Invalid amount. Only decimal numbers are allowed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _selectedExpense.Amount = Convert.ToDouble(AmountTextBox.Text);

                string connString = _configManager.GetConnectionString();

                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("UPDATE finance SET date = @Date, expense = @Expense, category = @Category, amount = @Amount WHERE id = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("Date", _selectedExpense.Date);
                        cmd.Parameters.AddWithValue("Expense", _selectedExpense.Expense);
                        cmd.Parameters.AddWithValue("Category", _selectedExpense.Category);
                        cmd.Parameters.AddWithValue("Amount", _selectedExpense.Amount);
                        cmd.Parameters.AddWithValue("Id", _selectedExpense.Id);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                DataUpdated?.Invoke(this, EventArgs.Empty);

                Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            string connString = _configManager.GetConnectionString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM finance WHERE id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("Id", _selectedExpense.Id);

                   await cmd.ExecuteNonQueryAsync();
                }
            }

            DataUpdated?.Invoke(this, EventArgs.Empty);

            Close();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool areFieldsFilled = AreFieldsFilled;
            UpdateButton.IsEnabled = areFieldsFilled;
            if (!areFieldsFilled)
            {
                UpdateButton.IsEnabled = false;
            }
        }

        private bool AreFieldsFilled
        {
            get
            {
                return !string.IsNullOrEmpty(DatePicker.Text) &&
                       !string.IsNullOrEmpty(ExpenseTextBox.Text) &&
                       !string.IsNullOrEmpty(CategoryTextBox.Text) &&
                       !string.IsNullOrEmpty(AmountTextBox.Text);
            }
        }
    }
}
