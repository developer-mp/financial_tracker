using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FinancialTracker
{
    public class TotalExpenses
    {
        private ObservableCollection<ExpenseItem> _expenseList;

        public TotalExpenses(ObservableCollection<ExpenseItem> expenseList)
        {
            _expenseList = expenseList;
        }

        public double CalculateTotalExpenses()
        {
            return _expenseList.Sum(expense => expense.Amount);
        }
    }
}
