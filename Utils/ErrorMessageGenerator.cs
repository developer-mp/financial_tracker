using FinancialTracker.Models;
using FinancialTracker.Service;
using System.Windows;

public static class ErrorMessageGenerator
{
    public static void ShowError(string errorName, ConfigService configService)
    {
        ErrorMessage errorMessage = configService.GetErrorMessage(errorName, false);
        MessageBox.Show(errorMessage.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void ShowSuccess(string errorName, ConfigService configService)
    {
        ErrorMessage successMessage = configService.GetErrorMessage(errorName, true);
        MessageBox.Show(successMessage.Success, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
