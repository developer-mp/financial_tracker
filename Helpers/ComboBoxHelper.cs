using FinancialTracker.Service;
using System.Collections.Generic;
using System.Windows.Controls;
using System;

public static class ComboBoxHelper
{
    public static void PopulateCategoryComboBox(ComboBox comboBox, ConfigService configManager, string selectedCategory = null)
    {
        try
        {
            List<string> categoryNames = configManager.GetCategoryNames();
            comboBox.ItemsSource = categoryNames;

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                comboBox.SelectedItem = selectedCategory;
            }
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error populating combo box: {ex.Message}");
        }
    }
}
