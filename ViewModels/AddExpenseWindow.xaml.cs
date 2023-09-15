using FinancialTracker.Models;
using FinancialTracker.Service;
using System;
using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public partial class AddExpenseWindow : Window
    {
        private MainWindow _mainWindow;
        private EnvService _envService;
        private ConfigService _configService;
        private ExpenseManager _expenseManager;
        private DataLoadingService _dataLoadingService;
        private string _connectionString;
        private ButtonStateHelper _buttonStateHelper;

        public AddExpenseWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _envService = new EnvService();
            _configService = new ConfigService();
            _dataLoadingService = new DataLoadingService();
            _expenseManager = new ExpenseManager(_configService, _dataLoadingService);
            _connectionString = _envService.GetConnectionString();
            _mainWindow = mainWindow;

            InitializeComboBox();
            InitializeButtonStateHelper();

            ExpenseTextBox.TextChanged += OnTextChanged;
            AmountTextBox.TextChanged += OnTextChanged;
        }

        private void InitializeComboBox()
        {
            ComboBoxHelper.PopulateCategoryComboBox(CategoryComboBox, _configService);
        }

        private void InitializeButtonStateHelper()
        {
            _buttonStateHelper = new ButtonStateHelper(SaveButton, ExpenseTextBox, AmountTextBox);
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string expenseText = ExpenseTextBox.Text;
                string amountText = AmountTextBox.Text;

                if (!Double.TryParse(amountText, out double amount))
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }

                ExpenseItem newExpense = _expenseManager.CreateNewExpense(Guid.NewGuid().ToString(),
    DatePicker.SelectedDate ?? DateTime.Now, expenseText, CategoryComboBox.SelectedItem.ToString(), amount);

                if (newExpense == null)
                {
                    return;
                }

                DbQuery insertDbQuery = _configService.GetDbQuery("AddExpense");
                _expenseManager.InsertExpenseToDatabase(_connectionString, insertDbQuery, newExpense);
                _mainWindow.expenseList.Add(newExpense);
                Close();
                ErrorMessageGenerator.ShowSuccess("AddNewRecord", _configService);
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("AddNewRecord", _configService);
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

