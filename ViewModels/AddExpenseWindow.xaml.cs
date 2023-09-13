using FinancialTracker.Models;
using FinancialTracker.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class AddExpenseWindow : Window
    {
        private MainWindow _mainWindow;
        private EnvManager _envManager;
        private ConfigManager _configManager;
        private DataLoadingService _dataLoadingService;
        private string _connectionString;
        private ButtonStateHelper _buttonStateHelper;

        public AddExpenseWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _envManager = new EnvManager();
            _configManager = new ConfigManager();
            _dataLoadingService = new DataLoadingService();
            _connectionString = _envManager.GetConnectionString();
            _mainWindow = mainWindow;
            PopulateCategoryComboBox();
            _buttonStateHelper = new ButtonStateHelper(SaveButton, ExpenseTextBox, AmountTextBox);

            ExpenseTextBox.TextChanged += OnTextChanged;
            AmountTextBox.TextChanged += OnTextChanged;
        }

        private void PopulateCategoryComboBox()
        {
            try
            {
                List<string> categoryNames = _configManager.GetCategoryNames();
                CategoryComboBox.ItemsSource = categoryNames;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error populating combo box: {ex.Message}");
            }
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
                    Category = CategoryComboBox.SelectedItem.ToString(),
                };

                if (!ValidationHelper.TryParseDouble(AmountTextBox, out double amount))
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configManager);
                    return;
                }

                newExpense.Amount = Convert.ToDouble(AmountTextBox.Text);
                DbQuery insertDbQuery = _configManager.GetDbQuery("AddExpenseData");
                _dataLoadingService.InsertExpense(_connectionString, insertDbQuery, newExpense);
                _mainWindow.expenseList.Add(newExpense);
                Close();

                ErrorMessageGenerator.ShowSuccess("AddNewRecord", _configManager);
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("AddNewRecord", _configManager);
                Console.WriteLine($"Error adding a new record: {ex.Message}");
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error clicking a cancel button: {ex.Message}");
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _buttonStateHelper.UpdateButtonState();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error updating button state: {ex.Message}");
            }
        }
    }
}
