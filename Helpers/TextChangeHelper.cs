using System.Windows.Controls;
using System;

public class TextChangeHandler
{
    private ButtonStateHelper _buttonStateHelper;

    public TextChangeHandler(ButtonStateHelper buttonStateHelper)
    {
        _buttonStateHelper = buttonStateHelper;
    }

    public void HandleTextChange(object sender, TextChangedEventArgs e)
    {
        try
        {
            _buttonStateHelper.UpdateButtonState();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating button state: {ex.Message}");
        }
    }
}
