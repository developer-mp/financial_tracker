using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FinancialTracker.Models;
using FinancialTracker.Service;
using FinancialTracker.Utils;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private EnvManager _envManager;
        private ConfigManager _configManager;
        private DataLoadingService _dataLoadingService;
        private string _connectionString;
        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();
        private SortingHandler _sortingHandler;

        public MainWindow()
        {
            InitializeComponent();
            _configManager = new ConfigManager();
            _dataLoadingService = new DataLoadingService();
            _envManager = new EnvManager();
            _connectionString = _envManager.GetConnectionString();
            TransactionListView.ItemsSource = expenseList;
            LoadData();
            LoadTotalExpenses();
            LoadTotalExpensesByCategory();
            GenerateChart();
            DataContext = this;
            _sortingHandler = new SortingHandler(this, TransactionListView);
            SetDefaultSorting();
        }

        private void SetDefaultSorting()
        {
            try
            {
                var defaultSorting = _configManager.GetDefaultSorting();
                _sortingHandler.SetDefaultSorting(defaultSorting.SortColumn, defaultSorting.SortDirection);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error default sorting: {ex.Message}");
            }
        }

        private void ColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var headerClicked = e.OriginalSource as GridViewColumnHeader;
                _sortingHandler.ColumnHeaderClicked(headerClicked);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error sorting column headers: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                expenseList.Clear();
                DbQuery DbQuery = _configManager.GetDbQuery("LoadFinanceData");
                ObservableCollection<ExpenseItem> loadedData = _dataLoadingService.LoadData(_connectionString, DbQuery);
                foreach (var expense in loadedData)
                {
                    expenseList.Add(expense);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void LoadTotalExpenses()
        {
            try
            {
                DbQuery DbQuery = _configManager.GetDbQuery("LoadTotalExpensesData");
                double totalExpenses = _dataLoadingService.LoadTotalExpenses(_connectionString, DbQuery);
                TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error loading total expenses: {ex.Message}");
            }
        }

        private List<ExpenseByCategory> LoadTotalExpensesByCategory()
        {
            try
            {
                DbQuery DbQuery = _configManager.GetDbQuery("LoadExpensesByCategoryData");
                List<ExpenseByCategory> expensesByCategory = _dataLoadingService.LoadExpensesByCategory(_connectionString, DbQuery);
                return expensesByCategory;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error loading total expenses by category: {ex.Message}");
                return null;
            }
        }

        private void TransactionListViewEdit(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (TransactionListView.SelectedItem != null)
                {
                    ExpenseItem selectedExpense = (ExpenseItem)TransactionListView.SelectedItem;
                    EditExpenseWindow editExpenseWindow = new EditExpenseWindow(selectedExpense);
                    editExpenseWindow.DataUpdated += EditExpenseWindowDataUpdated;
                    editExpenseWindow.ShowDialog();
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error displaying Edit View: {ex.Message}");
            }
        }

        private void EditExpenseWindowDataUpdated(object sender, EventArgs e)
        {
            try
            {
                expenseList.Clear();
                LoadData();
                LoadTotalExpenses();
                LoadTotalExpensesByCategory();
                GenerateChart();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error loading updated records: {ex.Message}");
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
                addExpenseWindow.ShowDialog();
                LoadTotalExpenses();
                LoadTotalExpensesByCategory();
                GenerateChart();
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error clicking Add button: {ex.Message}");
            }
        }

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfFilePath = null;

                if (ReportGenerator.SaveReport(out pdfFilePath))
                {
                    string totalExpensesText = TotalExpensesTextBlock.Text;
                    BitmapImage chartImage = (BitmapImage)ChartImage.Source;

                    ReportGenerator.CreateReport(pdfFilePath, totalExpensesText, chartImage);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error clicking Print button: {ex.Message}");
            }
        }

        private void GenerateChart()
        {
            try
            {
                string pythonDllPath = _envManager.GetPythonDLLPath();
                List<string> categoryColors = _configManager.GetCategoryColors();

                List<ExpenseByCategory> expensesByCategory = LoadTotalExpensesByCategory();

                ChartGenerator chartGenerator = new ChartGenerator(pythonDllPath, categoryColors);
                BitmapImage chartImage = chartGenerator.GenerateChart(expensesByCategory);

                if (chartImage != null)
                {
                    ChartImage.Source = chartImage;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error saving PDF report: {ex.Message}");
            }

        }
    }
}