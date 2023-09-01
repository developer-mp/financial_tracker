using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using Python.Runtime;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Markup;

namespace FinancialTracker
{
    public partial class MainWindow : Window
    {
        private ConfigurationManager _configManager;

        private DataLoadingService _dataLoadingService;

        private string _connectionString;

        public ObservableCollection<ExpenseItem> expenseList = new ObservableCollection<ExpenseItem>();

        public MainWindow()
        {
            InitializeComponent();
            _configManager = new ConfigurationManager();
            _connectionString = _configManager.GetConnectionString();
            _dataLoadingService = new DataLoadingService();
            TransactionListView.ItemsSource = expenseList;
            LoadData();
            LoadTotalExpenses();
            //LoadTotalExpensesByCategory();
            DataContext = this;
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

        //private void LoadTotalExpensesByCategory()
        //{
        //    QuerySettings querySettings = _configManager.GetQuerySettings("LoadExpensesByCategoryData");
        //    List<ExpenseByCategory> expensesByCategory = _dataLoadingService.LoadExpensesByCategory(_connectionString, querySettings);

        //    TotalExpensesByCategoryListView.ItemsSource = expensesByCategory;
        //}


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
            //LoadTotalExpensesByCategory();
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
            addExpenseWindow.ShowDialog();
            LoadTotalExpenses();
            //LoadTotalExpensesByCategory();
        }

        //private void PrintButtonClick(object sender, RoutedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start("python", "GeneratePieChart.py");
        //}

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            string pythonDllPath = _configManager.GetPythonDLLPath();

            if (!string.IsNullOrEmpty(pythonDllPath))
            {
                Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonDllPath);
                PythonEngine.Initialize();

                using (Py.GIL())
                {
                    dynamic np = Py.Import("numpy");
                    dynamic plt = Py.Import("matplotlib.pyplot");

                    dynamic sizes = np.array(new double[] { 15, 30, 45, 10 });
                    dynamic labels = new List<string> { "A", "B", "C", "D" };
                    dynamic colors = new List<string> { "gold", "yellowgreen", "lightcoral", "lightskyblue" };

                    dynamic wedges;
                    dynamic texts;
                    plt.pie(sizes, colors: colors, startangle: 140);
                    dynamic result = plt.pie(sizes, colors: colors, autopct: "%1.0f%%", startangle: 140);
                    wedges = result[0];
                    texts = result[1];

                    plt.legend(wedges, labels, loc: "center left", bbox_to_anchor: new double[] { 1, 0.5 });

                    dynamic io = Py.Import("io");
                    dynamic buf = io.BytesIO();
                    plt.savefig(buf, format: "png");
                    buf.seek(0);

                    byte[] chartBytes = buf.getvalue();

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(chartBytes);
                    bitmapImage.EndInit();

                    ChartImage.Source = bitmapImage;
                }
            }
            else
            {
                MessageBox.Show("Python DLL path not configured");
            }
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

    public class ExpenseByCategory
    {
        public string Category { get; set; }
        public double TotalAmount { get; set; }
    }
}