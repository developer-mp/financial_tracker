using FinancialTracker.Service;
using System;
using System.Windows;

namespace FinancialTracker
{
    public partial class FilterWindow : Window
    {
        private ConfigService _configService;

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string Category { get; private set; }
        public double MinAmount { get; private set; }
        public double MaxAmount { get; private set; }

        public FilterWindow()
        {
            InitializeComponent();
            _configService = new ConfigService();

            StartDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
            Category = "";
            MinAmount = 0;
            MaxAmount = double.MaxValue;

            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            ComboBoxHelper.PopulateCategoryComboBox(CategoryComboBox, _configService);
        }

        private void ApplyDateFilterClick(object sender, RoutedEventArgs e)
        {
            StartDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            EndDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;
            Category = CategoryComboBox.SelectedItem?.ToString() ?? "";

            string minAmountText = MinAmountTextBox.Text;
            string maxAmountText = MaxAmountTextBox.Text;

            if (string.IsNullOrEmpty(minAmountText))
            {
                MinAmount = 0;
            }
            else
            {
                if (double.TryParse(minAmountText, out double parsedMinAmount))
                {
                    MinAmount = parsedMinAmount;
                }
                else
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }
            }

            if (string.IsNullOrEmpty(maxAmountText))
            {
                MaxAmount = double.MaxValue;
            }
            else
            {
                if (double.TryParse(maxAmountText, out double parsedMaxAmount))
                {
                    MaxAmount = parsedMaxAmount;
                }
                else
                {
                    ErrorMessageGenerator.ShowError("ValidateAmount", _configService);
                    return;
                }
            }

            ApplyFilterRequested?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void ClearDateFilterClick(object sender, RoutedEventArgs e)
        {
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
            Category = "";
            MinAmount = 0;
            MaxAmount = double.MaxValue;

            ClearFilterRequested?.Invoke(this, EventArgs.Empty);
            Close();
        }

        public event EventHandler ApplyFilterRequested;
        public event EventHandler ClearFilterRequested;
    }
}

