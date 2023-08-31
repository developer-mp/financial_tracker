﻿using System;
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

        public MainWindow()
        {
            InitializeComponent();
            _configManager = new ConfigurationManager();
            _connectionString = _configManager.GetConnectionString();
            _dataLoadingService = new DataLoadingService();
            TransactionListView.ItemsSource = expenseList;
            LoadData();
            LoadTotalExpenses();
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
            double totalExpenses = _dataLoadingService.ExecuteScalarQuery(_connectionString, querySettings);

            TotalExpensesTextBlock.Text = $"{totalExpenses:N2}";
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
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            AddExpenseWindow addExpenseWindow = new AddExpenseWindow(this);
            addExpenseWindow.ShowDialog();
            LoadTotalExpenses();
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