using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinancialTracker
{
    public class CategoryTotalsCalculator
    {
        private ObservableCollection<ExpenseItem> _expenseList;

        public CategoryTotalsCalculator(ObservableCollection<ExpenseItem> expenseList)
        {
            _expenseList = expenseList ?? throw new ArgumentNullException(nameof(expenseList));
        }

        public Dictionary<string, double> CalculateCategoryTotals()
        {
            var categoryTotals = new Dictionary<string, double>();

            foreach (ExpenseItem expense in _expenseList)
            {
                if (categoryTotals.ContainsKey(expense.Category))
                {
                    categoryTotals[expense.Category] += expense.Amount;
                }
                else
                {
                    categoryTotals[expense.Category] = expense.Amount;
                }
            }

            return categoryTotals;
        }
    }
}
