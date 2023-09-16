using FinancialTracker.Models;
using FinancialTracker.Service;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class EditExpenseWindow : Window
    {
        private EnvService _envService;
        private ConfigService _configService;
        private DataLoadingService _dataLoadingService;
        private ExpenseItem _selectedExpense;
        private string _connectionString;
        private ButtonStateHelper _buttonStateHelper;

        public EditExpenseWindow(ExpenseItem selectedExpense)
        {
            InitializeComponent();

            _envService = new EnvService();
            _configService = new ConfigService();
            _dataLoadingService = new DataLoadingService();
            _connectionString = _envService.GetConnectionString();
            _selectedExpense = selectedExpense;

            InitializeComboBox();

            DatePicker.SelectedDate = _selectedExpense.Date;
            ExpenseTextBox.Text = _selectedExpense.Expense;
            AmountTextBox.Text = _selectedExpense.Amount.ToString();

            ExpenseTextBox.TextChanged += OnTextChanged;
            AmountTextBox.TextChanged += OnTextChanged;
        }

        public event EventHandler DataUpdated;

        private void InitializeComboBox()
        {
            ComboBoxHelper.PopulateCategoryComboBox(CategoryComboBox, _configService, _selectedExpense.Category);
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _selectedExpense.Date = DatePicker.SelectedDate ?? DateTime.Now;
                _selectedExpense.Expense = ExpenseTextBox.Text;
                _selectedExpense.Category = CategoryComboBox.Text;

                if (!Double.TryParse(AmountTextBox.Text, out double amount))
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }

                _selectedExpense.Amount = amount;
                DbQuery updateDbQuery = _configService.GetDbQuery("UpdateExpense");
                _dataLoadingService.UpdateExpense(_connectionString, updateDbQuery, _selectedExpense);
                DataUpdated?.Invoke(this, EventArgs.Empty);
                Close();

                ErrorMessageGenerator.ShowSuccess("UpdateRecord", _configService);
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("UpdateRecord", _configService);
                Console.WriteLine($"Error updating record: {ex.Message}");
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DbQuery deleteDbQuery = _configService.GetDbQuery("DeleteExpense");
                _dataLoadingService.DeleteExpense(_connectionString, deleteDbQuery, _selectedExpense);
                DataUpdated?.Invoke(this, EventArgs.Empty);
                Close();

                ErrorMessageGenerator.ShowSuccess("DeleteRecord", _configService);
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("DeleteRecord", _configService);
                Console.WriteLine($"Error deleting record: {ex.Message}");
            }

        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _buttonStateHelper = new ButtonStateHelper(UpdateButton, ExpenseTextBox, AmountTextBox);
            }
            catch(Exception ex)
            {
                ErrorMessageGenerator.ShowError("GeneralError", _configService);
                Console.WriteLine($"Error updating button state: {ex.Message}");
            }
        }
    }
}
