using System.Collections.ObjectModel;

namespace FinancialTracker
{
    public class TotalExpensesCalculator
    {
        public static double CalculateTotalExpenses(ObservableCollection<ExpenseItem> expenses)
        {
            double totalExpenses = 0;
            foreach (ExpenseItem expense in expenses)
            {
                totalExpenses += expense.Amount;
            }
            return totalExpenses;
        }
    }
}
