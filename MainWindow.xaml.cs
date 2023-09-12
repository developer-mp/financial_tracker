using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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
        private SortingHelper _sortingHelper;

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
            _sortingHelper = new SortingHelper(this, TransactionListView);
            SetDefaultSorting();
        }

        private void SetDefaultSorting()
        {
            var defaultSorting = _configManager.GetDefaultSorting();
            _sortingHelper.SetDefaultSorting(defaultSorting.SortColumn, defaultSorting.SortDirection);
        }

        private void ColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            _sortingHelper.ColumnHeaderClicked(headerClicked);
        }

        private void LoadData()
        {
            expenseList.Clear();
            QuerySettings querySettings = _configManager.GetQuerySettings("LoadFinanceData");
            ObservableCollection<ExpenseItem> loadedData = _dataLoadingService.LoadData(_connectionString, querySettings);
            foreach (var expense in loadedData)
            {
                expenseList.Add(expense);
            }
        }

        private void LoadTotalExpenses()
        {
            QuerySettings querySettings = _configManager.GetQuerySettings("LoadTotalExpensesData");
            double totalExpenses = _dataLoadingService.LoadTotalExpenses(_connectionString, querySettings);
            TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
        }

        private List<ExpenseByCategory> LoadTotalExpensesByCategory()
        {
            QuerySettings querySettings = _configManager.GetQuerySettings("LoadExpensesByCategoryData");
            List<ExpenseByCategory> expensesByCategory = _dataLoadingService.LoadExpensesByCategory(_connectionString, querySettings);
            return expensesByCategory;

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
            LoadTotalExpenses();
            LoadTotalExpensesByCategory();
            GenerateChart();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
            addExpenseWindow.ShowDialog();
            LoadTotalExpenses();
            LoadTotalExpensesByCategory();
            GenerateChart();
        }

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            string pdfFilePath = null;

            if (PdfGenerator.SaveReport(out pdfFilePath))
            {
                string totalExpensesText = TotalExpensesTextBlock.Text;
                BitmapImage chartImage = (BitmapImage)ChartImage.Source;

                PdfGenerator.GenerateReport(pdfFilePath, totalExpensesText, chartImage);
            }
        }

        private void GenerateChart()
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
            else
            {
                MessageBox.Show("Failed to generate the chart");
            }
        }
    }
}