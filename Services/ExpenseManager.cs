using FinancialTracker.Models;
using FinancialTracker.Service;
using System;
using System.Windows.Controls;

public class ExpenseManager
{
    private ConfigManager _configManager;
    private DataLoadingService _dataLoadingService;

    public ExpenseManager(ConfigManager configManager, DataLoadingService dataLoadingService)
    {
        _configManager = configManager;
        _dataLoadingService = dataLoadingService;
    }

    public ExpenseItem CreateNewExpense(string id, DateTime date, string expenseText, string category, double amount)
    {
        try
        {
            //Guid newGuid = Guid.NewGuid();
            //string newId = newGuid.ToString();

            ExpenseItem newExpense = new ExpenseItem
            {
                //Id = newId,
                //Date = DatePicker.SelectedDate ?? DateTime.Now,
                //Expense = expenseText,
                //Category = categoryComboBox.SelectedItem.ToString(),

                Id = id,
                Date = date,
                Expense = expenseText,
                Category = category,
                Amount = amount,
            };

            //if (!ValidationHelper.TryParseDouble(amountText, out double amount))
            //{
            //    return null;
            //}

            //newExpense.Amount = Convert.ToDouble(AmountTextBox.Text);
            return newExpense;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating a new expense: {ex.Message}");
            return null;
        }
    }

    public void InsertExpenseToDatabase(string connectionString, DbQuery insertDbQuery, ExpenseItem newExpense)
    {
        try
        {
            _dataLoadingService.InsertExpense(connectionString, insertDbQuery, newExpense);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting the new expense: {ex.Message}");
        }
    }
}
