using System.Windows;

namespace FinancialTracker
{
    public partial class ModifyExpenseWindow : Window
    {
        private ExpenseItem _selectedExpense;

        public ModifyExpenseWindow(ExpenseItem selectedExpense)
        {
            InitializeComponent();
            _selectedExpense = selectedExpense;

            // Set up the UI elements for editing the selectedExpense
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            // Update the selectedExpense with new values
            // Update the UI and the database accordingly
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            // Delete the selectedExpense from the collection
            // Update the UI and the database accordingly
        }
    }
}
