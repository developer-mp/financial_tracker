using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using FinancialTracker.Service;
using FinancialTracker.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private EnvManager _envManager;
        private ConfigManager _configManager;
        private DataLoadingService _dataLoadingService;
        private string _connectionString;
        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

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
            SetDefaultSorting();
        }

        private void SetDefaultSorting()
        {
            GridView gridView = TransactionListView.View as GridView;

            if (gridView != null)
            {
                var dateColumn = gridView.Columns.FirstOrDefault(column => column.Header.ToString() == "Date");

                if (dateColumn != null)
                {
                    dateColumn.HeaderTemplate = Resources["HeaderArrowDown"] as DataTemplate;
                }
            }

            Sort("Date", ListSortDirection.Descending);
        }

        private void ColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderArrowDown"] as DataTemplate;
                    }

                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(TransactionListView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
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
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Files|*.pdf";
                saveFileDialog.Title = "Save PDF Report";
                saveFileDialog.FileName = "SummaryReport.pdf";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string pdfFilePath = saveFileDialog.FileName;

                    GenerateAndSavePDF(pdfFilePath);

                    MessageBox.Show("PDF report saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateAndSavePDF(string pdfFilePath)
        {
            try
            {
                Document doc = new Document();
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(pdfFilePath, FileMode.Create));
                doc.Open();

                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell(new Phrase($"Total Expenses: ${TotalExpensesTextBlock.Text}"));
                cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.FixedHeight = 50f;
                table.AddCell(cell);

                PdfPCell chartCell = new PdfPCell();
                BitmapImage chartImage = (BitmapImage)ChartImage.Source;
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(chartImage));

                string tempImagePath = Path.GetTempFileName() + ".jpg";

                using (var stream = new FileStream(tempImagePath, FileMode.Create))
                {
                    encoder.Save(stream);
                }

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(tempImagePath);
                image.ScaleAbsolute(500f, 400f);
                chartCell.AddElement(image);
                table.AddCell(chartCell);

                doc.Add(table);
                File.Delete(tempImagePath);
                doc.Close();

                MessageBox.Show("PDF report generated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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