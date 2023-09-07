using FinancialTracker.Service;
using Npgsql;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class AddExpenseWindow : Window
    {
        private MainWindow _mainWindow;
        private EnvManager _configManager;

        public AddExpenseWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _configManager = new EnvManager();
            _mainWindow = mainWindow;

        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Guid newGuid = Guid.NewGuid();
                string newId = newGuid.ToString();

                ExpenseItem newExpense = new ExpenseItem
                {
                    Id = newId,
                    Date = DatePicker.SelectedDate ?? DateTime.Now,
                    Expense = ExpenseTextBox.Text,
                    Category = CategoryTextBox.Text,
                };

                if (!double.TryParse(AmountTextBox.Text, out double amount))
                {
                    MessageBox.Show("Invalid amount. Only decimal numbers are allowed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                newExpense.Amount = amount;

                string connString = _configManager.GetConnectionString();

                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("INSERT INTO finance (id, date, expense, category, amount) VALUES (@Id, @Date, @Expense, @Category, @Amount)", conn))
                    {
                        cmd.Parameters.AddWithValue("Id", newExpense.Id);
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
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool areFieldsFilled = AreFieldsFilled;
            SaveButton.IsEnabled = areFieldsFilled;
            if (areFieldsFilled)
            {
                SaveButton.IsEnabled = true;
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
