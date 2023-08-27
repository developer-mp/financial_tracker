using LiveChartsCore;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView;

namespace FinancialTracker
{
    public partial class Chart : ObservableObject
    {
        private int _groceries = 200; 
        private int _transportation = 100; 
        private int _housing = 300;
        private int _utilities = 50;
        private int _healthcare = 300;
        private int _clothing = 300;
        private int _entertainment = 300;
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

        public int Entertainment
        {
            get => _entertainment;
            set => SetProperty(ref _entertainment, value);
        }

        public int Miscellaneous
        {
            get => _miscellaneous;
            set => SetProperty(ref _miscellaneous, value);
        }

        public IEnumerable<ISeries> Series { get; set; }

        public Chart()
        {

            var groceriesSeries = new PieSeries<int>
            {
                Values = new[] { Groceries },
                Name = "Groceries",
                InnerRadius = 50
            };

            var transportationSeries = new PieSeries<int>
            {
                Values = new[] { Transportation },
                Name = "Transportation",
                InnerRadius = 50
            };

            var housingSeries = new PieSeries<int>
            {
                Values = new[] { Housing },
                Name = "Housing",
                InnerRadius = 50
            };

            var utilitiesSeries = new PieSeries<int>
            {
                Values = new[] { Utilities },
                Name = "Utilities",
                InnerRadius = 50
            };

            var healthcareSeries = new PieSeries<int>
            {
                Values = new[] { Healthcare },
                Name = "Healthcare",
                InnerRadius = 50
            };

            var clothingSeries = new PieSeries<int>
            {
                Values = new[] { Clothing },
                Name = "Clothing",
                InnerRadius = 50
            };

            var entertainmentSeries = new PieSeries<int>
            {
                Values = new[] { Transportation },
                Name = "Entertainment",
                InnerRadius = 50
            };

            var miscellaneousSeries = new PieSeries<int>
            {
                Values = new[] { Miscellaneous },
                Name = "Miscellaneous",
                InnerRadius = 50
            };

            Series = new List<ISeries> { groceriesSeries, transportationSeries, housingSeries, utilitiesSeries, healthcareSeries, clothingSeries, entertainmentSeries, miscellaneousSeries };

        }
    }
}
