using FinancialTracker.Models;
using FinancialTracker.Service;
using System;
using System.Windows;

namespace FinancialTracker
{
    public partial class FilterWindow : Window
    {
        private ConfigService _configService;
        private Filter _selectedFilter;

        public FilterWindow(Filter selectedFilter)
        {
            InitializeComponent();
            _configService = new ConfigService();
            _selectedFilter = selectedFilter;

            InitializeComboBox();

            if (_selectedFilter != null)
            {
                StartDatePicker.SelectedDate = _selectedFilter.StartDate;
                EndDatePicker.SelectedDate = _selectedFilter.EndDate;
                CategoryComboBox.SelectedItem = _selectedFilter.Category;
                MinAmountTextBox.Text = _selectedFilter.MinAmount.ToString();
                MaxAmountTextBox.Text = _selectedFilter.MaxAmount.ToString();
            }
        }

        private void InitializeComboBox()
        {
            ComboBoxHelper.PopulateCategoryComboBox(CategoryComboBox, _configService);
        }

        private void ApplyDateFilterClick(object sender, RoutedEventArgs e)
        {
            string minAmountText = MinAmountTextBox.Text;
            string maxAmountText = MaxAmountTextBox.Text;

                if (double.TryParse(minAmountText, out double parsedMinAmount))
                {
                    _selectedFilter.MinAmount = parsedMinAmount;
                }
                else
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }

                if (double.TryParse(maxAmountText, out double parsedMaxAmount))
                {
                    _selectedFilter.MaxAmount = parsedMaxAmount;
                }
                else
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }

            ApplyFilterRequested?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void ClearDateFilterClick(object sender, RoutedEventArgs e)
        {
            ClearFilterRequested?.Invoke(this, EventArgs.Empty);
            Close();
        }

        public event EventHandler ApplyFilterRequested;
        public event EventHandler ClearFilterRequested;
    }
}

