using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private ConfigurationManager _configManager;

        private DataLoadingService _dataLoadingService;

        private string _connectionString;

        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        private void UpdateTotalExpensesTextBlock()
        {
            double totalExpenses = TotalExpensesCalculator.CalculateTotalExpenses(expenseList);
            TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
        }
        public MainWindow()
        {
            InitializeComponent();
            _configManager = new ConfigurationManager();
            _connectionString = _configManager.GetConnectionString();
            _dataLoadingService = new DataLoadingService();
            TransactionListView.ItemsSource = expenseList;
            LoadData();
            DataContext = this;
            UpdateTotalExpensesTextBlock();
        }
        private void LoadData()
        {
            expenseList.Clear();
            ObservableCollection<ExpenseItem> loadedData = _dataLoadingService.LoadData(_connectionString);
            foreach (var expense in loadedData)
            {
                expenseList.Add(expense);
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