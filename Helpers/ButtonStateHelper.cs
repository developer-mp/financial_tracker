namespace FinancialTracker.ButtonState
{

    using System.Windows.Controls;

    public class ButtonStateHelper
    {
        public ButtonStateHelper(Button button, TextBox expenseTextBox, TextBox amountTextBox)
        {
            UpdateButtonState(button, expenseTextBox, amountTextBox);
        }

        public static void UpdateButtonState(Button button, TextBox expenseTextBox, TextBox amountTextBox)
        {
            bool areFieldsFilled = !string.IsNullOrEmpty(expenseTextBox.Text) &&
                                   !string.IsNullOrEmpty(amountTextBox.Text);

            button.IsEnabled = areFieldsFilled;
        }
    }
}
