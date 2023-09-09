using System.Windows.Controls;

public class ButtonStateHelper
{
    private Button _button;
    private TextBox _expenseTextBox;
    private TextBox _amountTextBox;

    public ButtonStateHelper(Button button, TextBox expenseTextBox, TextBox amountTextBox)
    {
        _button = button;
        _expenseTextBox = expenseTextBox;
        _amountTextBox = amountTextBox;
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        bool areFieldsFilled = !string.IsNullOrEmpty(_expenseTextBox.Text) &&
                               !string.IsNullOrEmpty(_amountTextBox.Text);

        _button.IsEnabled = areFieldsFilled;
    }
}
