using System;
using System.Windows.Controls;

public class ButtonStateHelper
{
    public ButtonStateHelper(Button button, TextBox expenseTextBox, TextBox amountTextBox)
    {
            UpdateButtonState(button, expenseTextBox, amountTextBox);
    }

    public void UpdateButtonState(Button button, TextBox expenseTextBox, TextBox amountTextBox)
    {
        bool areFieldsFilled = !string.IsNullOrEmpty(expenseTextBox.Text) &&
                               !string.IsNullOrEmpty(amountTextBox.Text);

        button.IsEnabled = areFieldsFilled;
    }
}
