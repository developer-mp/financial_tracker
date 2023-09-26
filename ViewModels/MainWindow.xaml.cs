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
using Microsoft.Win32;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private EnvService _envService;
        private ConfigService _configService;
        private DataLoadingService _dataLoadingService;
        private string _connectionString;
        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();
        private SortingHandler _sortingHandler;
        private Filter _selectedFilter;

        private DateTime startDate = DateTime.MinValue;
        private DateTime endDate = DateTime.MaxValue;
        private string category = "";
        private double minAmount = 0;
        private double maxAmount = double.MaxValue;

        public MainWindow()
        {
            InitializeComponent();

            _envService = new EnvService();
            _configService = new ConfigService();
            _dataLoadingService = new DataLoadingService();
            _sortingHandler = new SortingHandler();
            _connectionString = _envService.GetConnectionString();
            TransactionListView.ItemsSource = expenseList;

            _selectedFilter = new Filter
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                Category = "",
                MinAmount = 0,
                MaxAmount = 0
            };

            LoadExpenses(startDate, endDate, category, minAmount, maxAmount);
            LoadTotalExpenses(startDate,endDate);
            LoadTotalExpensesByCategory(startDate, endDate);
            GenerateChart();
            SetDefaultSorting();

            DataContext = this;
        }

        private void SetDefaultSorting()
        {
            try
            {
                var defaultSorting = _configService.GetDefaultSorting();
                _sortingHandler.SetDefaultSorting(TransactionListView, this, defaultSorting.SortColumn, defaultSorting.SortDirection);
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
                _sortingHandler.ColumnHeaderClicked(TransactionListView, this, headerClicked);
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("GeneralError", _configService);
                Console.WriteLine($"Error sorting column headers: {ex.Message}");
            }
        }

        private void LoadExpenses(DateTime startDate, DateTime endDate, string category, double minAmount, double maxAmount)
        {
            try
            {
                expenseList.Clear();
                DbQuery DbQuery = _configService.GetDbQuery("LoadExpenses");
                ObservableCollection<ExpenseItem> loadedData = _dataLoadingService.LoadExpenses(_connectionString, DbQuery, startDate, endDate, category, minAmount, maxAmount);

                foreach (var expense in loadedData)
                {
                    expenseList.Add(expense);
                }
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("LoadData", _configService);
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void LoadTotalExpenses(DateTime startDate, DateTime endDate)
        {
            try
            {
                DbQuery DbQuery = _configService.GetDbQuery("LoadTotalExpenses");
                double totalExpenses = _dataLoadingService.LoadTotalExpenses(_connectionString, DbQuery, startDate, endDate);
                TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("LoadData", _configService);
                Console.WriteLine($"Error loading total expenses: {ex.Message}");
            }
        }

        private List<ExpenseByCategory> LoadTotalExpensesByCategory(DateTime startDate, DateTime endDate)
        {
            try
            {
                DbQuery DbQuery = _configService.GetDbQuery("LoadExpensesByCategory");

                List<ExpenseByCategory> expensesByCategory = _dataLoadingService.LoadExpensesByCategory(_connectionString, DbQuery, startDate, endDate);
                return expensesByCategory;
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("LoadData", _configService);
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
                ErrorMessageGenerator.ShowError("LoadData", _configService);
                Console.WriteLine($"Error displaying Edit View: {ex.Message}");
            }
        }

        private void EditExpenseWindowDataUpdated(object sender, EventArgs e)
        {
            try
            {
                expenseList.Clear();
                LoadExpenses(startDate, endDate, category, minAmount, maxAmount);
                LoadTotalExpenses(startDate, endDate);
                LoadTotalExpensesByCategory(startDate, endDate);
                GenerateChart();
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("LoadData", _configService);
                Console.WriteLine($"Error loading updated records: {ex.Message}");
            }
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
                addExpenseWindow.ShowDialog();
                LoadTotalExpenses(startDate, endDate);
                LoadTotalExpensesByCategory(startDate, endDate);
                GenerateChart();
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("GeneralError", _configService);
                Console.WriteLine($"Error clicking Add button: {ex.Message}");
            }
        }

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfFilePath = null;

                if (SaveReport(out pdfFilePath, _configService))
                {
                    string totalExpensesText = TotalExpensesTextBlock.Text;
                    BitmapImage chartImage = (BitmapImage)ChartImage.Source;

                    ReportGenerator.CreateReport(pdfFilePath, totalExpensesText, chartImage);
                }
            }
            catch (FormatException ex)
            {
                ErrorMessageGenerator.ShowError("GeneralError", _configService);
                Console.WriteLine($"Error clicking Print button: {ex.Message}");
            }
        }

        private void GenerateChart()
        {
            try
            {
                string pythonDllPath = _envService.GetPythonDLLPath();
                List<string> categoryColors = _configService.GetCategoryColors();

                List<ExpenseByCategory> expensesByCategory = LoadTotalExpensesByCategory(startDate, endDate);

                ChartGenerator chartGenerator = new ChartGenerator();
                BitmapImage chartImage = chartGenerator.GenerateChart(expensesByCategory, pythonDllPath, categoryColors);

                if (chartImage != null)
                {
                    ChartImage.Source = chartImage;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error generating the report: {ex.Message}");
            }
        }

        private static bool SaveReport(out string pdfFilePath, ConfigService configManager)
        {
            pdfFilePath = null;

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Save PDF Report";
                saveFileDialog.FileName = "SummaryReport.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    pdfFilePath = saveFileDialog.FileName;
                    ErrorMessageGenerator.ShowSuccess("SaveReport", configManager);
                    return true;
                }        
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving the report: {ex.Message}");
            }

            ErrorMessageGenerator.ShowError("SaveReport", configManager);
            return false;
        }

        private void OpenFilterWindowClick(object sender, RoutedEventArgs e)
        {
            FilterWindow filterWindow = new FilterWindow(new Filter
            {
                StartDate = _selectedFilter.StartDate,
                EndDate = _selectedFilter.EndDate,
                Category = _selectedFilter.Category,
                MinAmount = _selectedFilter.MinAmount,
                MaxAmount = _selectedFilter.MaxAmount
            });

            filterWindow.ApplyFilterRequested += (s, args) => ApplyFilter(filterWindow);
            filterWindow.ClearFilterRequested += (s, args) => ClearFilter(filterWindow);

            filterWindow.ShowDialog();
        }

        private void ApplyFilter(FilterWindow filterWindow)
        {
            _selectedFilter.StartDate = filterWindow.StartDatePicker.SelectedDate ?? DateTime.MinValue;
            _selectedFilter.EndDate = filterWindow.EndDatePicker.SelectedDate ?? DateTime.MaxValue;
            _selectedFilter.Category = filterWindow.CategoryComboBox.SelectedItem?.ToString() ?? "";
            _selectedFilter.MinAmount = double.TryParse(filterWindow.MinAmountTextBox?.Text, out double min) ? min : 0;
            _selectedFilter.MaxAmount = double.TryParse(filterWindow.MaxAmountTextBox?.Text, out double max) ? max : double.MaxValue;

            LoadExpenses(_selectedFilter.StartDate, _selectedFilter.EndDate, _selectedFilter.Category, _selectedFilter.MinAmount, _selectedFilter.MaxAmount);
        }

        private void ClearFilter(FilterWindow filterWindow)
        {
            _selectedFilter.StartDate = DateTime.Now;
            _selectedFilter.EndDate = DateTime.Now;
            _selectedFilter.Category = "";
            _selectedFilter.MinAmount = 0;
            _selectedFilter.MaxAmount = 0;

            LoadExpenses(startDate, endDate, category, minAmount, maxAmount);
        }
    }
}