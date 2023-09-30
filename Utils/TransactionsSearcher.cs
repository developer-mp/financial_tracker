using FinancialTracker.Models;
using System;
using System.Collections.ObjectModel;

namespace FinancialTracker
{
    public class TransactionsSearcher
    {
        public static void PerformTransactionsSearch(string searchText, ObservableCollection<ExpenseItem> expenseList)
        {
            var filteredTransactions = new ObservableCollection<ExpenseItem>();

            foreach (var transaction in expenseList)
            {
                var dateParts = transaction.Date.ToString("MMM d").Split(' ');

                string transactionMonth = dateParts[0].Trim();
                string transactionDate = dateParts[1].Trim();

                if (transactionMonth.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    transactionDate.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    transaction.Expense.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    transaction.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    transaction.Amount.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    filteredTransactions.Add(transaction);
                }
            }

            SearchCompleted?.Invoke(filteredTransactions);
        }

        public static event Action<ObservableCollection<ExpenseItem>> SearchCompleted;
    }
}
