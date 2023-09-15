using System;
using System.Windows.Controls;

public class ButtonStateHelper
{
    public ButtonStateHelper(Button button, TextBox expenseTextBox, TextBox amountTextBox)
    {
        try
        {
            UpdateButtonState(button, expenseTextBox, amountTextBox);
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error updating button state helper: {ex.Message}");
        }
    }

    public void UpdateButtonState(Button button, TextBox expenseTextBox, TextBox amountTextBox)
    {
        bool areFieldsFilled = !string.IsNullOrEmpty(expenseTextBox.Text) &&
                               !string.IsNullOrEmpty(amountTextBox.Text);

        button.IsEnabled = areFieldsFilled;
    }
}
