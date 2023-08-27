using LiveChartsCore;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace FinancialTracker
{
    public partial class Chart : ObservableObject
    {
        private int _groceries = 200; 
        private int _transportation = 100; 
        private int _entertainment = 300;
        private int _housing = 300;
        private int _utilities = 50;
        private int _healthcare = 300;
        private int _clothing = 300;
        private int _miscellaneous = 70;

        public int Groceries
        {
            get => _groceries;
            set => SetProperty(ref _groceries, value);
        }

        public int Transportation
        {
            get => _transportation;
            set => SetProperty(ref _transportation, value);
        }

        public int Entertainment
        {
            get => _entertainment;
            set => SetProperty(ref _entertainment, value);
        }

        public int Housing
        {
            get => _housing;
            set => SetProperty(ref _housing, value);
        }

        public int Utilities
        {
            get => _utilities;
            set => SetProperty(ref _utilities, value);
        }

        public int Healthcare
        {
            get => _healthcare;
            set => SetProperty(ref _healthcare, value);
        }

        public int Clothing
        {
            get => _clothing;
            set => SetProperty(ref _clothing, value);
        }

        public int Miscellaneous
        {
            get => _miscellaneous;
            set => SetProperty(ref _miscellaneous, value);
        }

        public IEnumerable<ISeries> Series { get; set; }

        public Chart()
        {

            Series = new[] { Groceries, Transportation, Entertainment, Housing, Utilities, Healthcare, Clothing, Miscellaneous }.AsPieSeries((value, series) =>
            {
                series.InnerRadius = 50;
                series.Name = "Food";
            });
        }
    }
}
