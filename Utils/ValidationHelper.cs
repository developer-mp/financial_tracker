using System.Windows;
using System.Windows.Controls;

public static class ValidationHelper
{
    public static bool TryParseDouble(TextBox textBox, out double result)
    {
        if (double.TryParse(textBox.Text, out result))
        {
            return true;
        }
        else
        {
            MessageBox.Show("Invalid amount. Only decimal numbers are allowed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }
}
