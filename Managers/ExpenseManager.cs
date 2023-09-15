using FinancialTracker.Models;
using System;

public class ExpenseManager
{
    public ExpenseItem CreateNewExpense(string id, DateTime date, string expenseText, string category, double amount)
    {
        try
        {

            ExpenseItem newExpense = new ExpenseItem
            {

                Id = id,
                Date = date,
                Expense = expenseText,
                Category = category,
                Amount = amount,
            };

            return newExpense;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating a new expense: {ex.Message}");
            return null;
        }
    }

    public void InsertNewExpense(DataLoadingService dataLoadingService, string connectionString, DbQuery insertDbQuery, ExpenseItem newExpense)
    {
        try
        {
            dataLoadingService.AddExpense(connectionString, insertDbQuery, newExpense);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting a new expense: {ex.Message}");
        }
    }
}
