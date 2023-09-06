using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using Python.Runtime;

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
            GenerateChart();
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

        private void PrintButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private void GenerateChart()
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

                    dynamic sizes = np.array(new double[] { 100, 30, 150, 60, 180, 70, 20, 10 });

                    var groceries = "Groceries" + " " + "$100";
                    var transportation = "Transportation" + " " + "$30";
                    var housing = "Housing" + " " + "$150";
                    var utilities = "Utilities" + " " + "$60";
                    var healthcare = "Healthcare" + " " + "$180";
                    var clothing = "Clothing" + " " + "$70";
                    var entertainment = "Entertainment" + " " + "$20";
                    var miscellaneous = "Miscellaneous" + " " + "$10";

                    dynamic labels = new List<string> { groceries, transportation, housing, utilities, healthcare, clothing, entertainment, miscellaneous };
                    dynamic colors = new List<string> { "#449E48", "#06CCB0", "#FF817E", "#F58216", "#FED679", "#866FFD",  "#3388FF", "#82D5F9" };

                    plt.figure().set_figwidth(9);

                    dynamic wedges;
                    //dynamic texts;
                    plt.pie(sizes, colors: colors, startangle: 140);
                    dynamic result = plt.pie(sizes, colors: colors, autopct: "%1.0f%%", startangle: 140);
                    wedges = result[0];
                    //texts = result[1];

                    plt.legend(wedges, labels, loc: "center left", bbox_to_anchor: new double[] { 1, 0.5 }, fontsize: 12);

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