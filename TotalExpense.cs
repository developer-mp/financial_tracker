using System.Collections.ObjectModel;
using System.Linq;

namespace FinancialTracker
{
    public class TotalExpense
    {
        private ObservableCollection<ExpenseItem> _expenseList;

        public TotalExpense(ObservableCollection<ExpenseItem> expenseList)
        {
            _expenseList = expenseList;
        }

        public double CalculateTotalExpenses()
        {
            return _expenseList.Sum(expense => expense.Amount);
        }
    }
}
