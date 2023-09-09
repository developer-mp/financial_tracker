using FinancialTracker.Service;
using FinancialTracker.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class EditExpenseWindow : Window
    {
        private EnvManager _envManager;
        private ConfigManager _configManager;
        private DataLoadingService _dataLoadingService;
        private ExpenseItem _selectedExpense;
        private string _connectionString;

        public EditExpenseWindow(ExpenseItem selectedExpense)
        {
            InitializeComponent();
            _envManager = new EnvManager();
            _configManager = new ConfigManager();
            _dataLoadingService = new DataLoadingService();
            _connectionString = _envManager.GetConnectionString();
            _selectedExpense = selectedExpense;

            DatePicker.SelectedDate = _selectedExpense.Date;
            ExpenseTextBox.Text = _selectedExpense.Expense;
            CategoryTextBox.Text = _selectedExpense.Category;
            AmountTextBox.Text = _selectedExpense.Amount.ToString();
        }

        public event EventHandler DataUpdated;

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedExpense.Date = DatePicker.SelectedDate ?? DateTime.Now;
                _selectedExpense.Expense = ExpenseTextBox.Text;
                _selectedExpense.Category = CategoryTextBox.Text;

                if (!double.TryParse(AmountTextBox.Text, out double amount))
                {
                    MessageBox.Show("Invalid amount. Only decimal numbers are allowed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _selectedExpense.Amount = Convert.ToDouble(AmountTextBox.Text);
                QuerySettings updateQuerySettings = _configManager.GetQuerySettings("UpdateExpenseData");
                _dataLoadingService.UpdateExpense(_connectionString, updateQuerySettings, _selectedExpense);
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
            QuerySettings deleteQuerySettings = _configManager.GetQuerySettings("DeleteExpenseData");
            _dataLoadingService.DeleteExpense(_connectionString, deleteQuerySettings, _selectedExpense);
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
