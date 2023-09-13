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
            return false;
        }
    }
}