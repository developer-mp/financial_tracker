using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace FinancialTracker
{
    public partial class Chart : ObservableObject
    {
            // you can convert any array, list or IEnumerable<T> to a pie series collection:
            public IEnumerable<ISeries> Series { get; set; } =
                new[] { 2, 4, 1, 4, 3 }.AsPieSeries((value, series) =>
                {
                    series.InnerRadius = 50;
                });
        }
        //    public ISeries[] Series { get; set; }

        //    public ViewModel(ObservableCollection<ExpenseItem> expenses)
        //    {
        //        var categoryTotals = expenses.GroupBy(expense => expense.Category)
        //                                     .Select(group => new
        //                                     {
        //                                         Category = group.Key,
        //                                         TotalAmount = group.Sum(expense => expense.Amount)
        //                                     })
        //                                     .ToList();

        //        // Create a list of PieSeries with data
        //        var pieSeries = categoryTotals.Select(item =>
        //            new PieSeries<double>
        //            {
        //                Values = new List<double> { item.TotalAmount },
        //            }).ToArray();

        //        Series = pieSeries;
        //    }
        //}
    }
