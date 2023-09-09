using FinancialTracker.Service;
using FinancialTracker.Utils;
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
            List<string> categoryNames = _configManager.GetCategoryNames();
            CategoryComboBox.ItemsSource = categoryNames;
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
                    return;
                }

                newExpense.Amount = Convert.ToDouble(AmountTextBox.Text);
                QuerySettings insertQuerySettings = _configManager.GetQuerySettings("AddExpenseData");
                _dataLoadingService.InsertExpense(_connectionString, insertQuerySettings, newExpense);
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
            _buttonStateHelper.UpdateButtonState();
        }
    }
}
