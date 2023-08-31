//using LiveChartsCore;
//using System.Collections.Generic;
//using CommunityToolkit.Mvvm.ComponentModel;
//using LiveChartsCore.SkiaSharpView.Extensions;
//using LiveChartsCore.SkiaSharpView;

//namespace FinancialTracker
//{
//    public partial class Chart : ObservableObject
//    {
//        private int _groceries = 200; 
//        private int _transportation = 100; 
//        private int _housing = 300;
//        private int _utilities = 50;
//        private int _healthcare = 300;
//        private int _clothing = 300;
//        private int _entertainment = 300;
//        private int _miscellaneous = 70;

//        public int Groceries
//        {
//            get => _groceries;
//            set => SetProperty(ref _groceries, value);
//        }

//        public int Transportation
//        {
//            get => _transportation;
//            set => SetProperty(ref _transportation, value);
//        }

//        public int Housing
//        {
//            get => _housing;
//            set => SetProperty(ref _housing, value);
//        }

//        public int Utilities
//        {
//            get => _utilities;
//            set => SetProperty(ref _utilities, value);
//        }

//        public int Healthcare
//        {
//            get => _healthcare;
//            set => SetProperty(ref _healthcare, value);
//        }

//        public int Clothing
//        {
//            get => _clothing;
//            set => SetProperty(ref _clothing, value);
//        }

//        public int Entertainment
//        {
//            get => _entertainment;
//            set => SetProperty(ref _entertainment, value);
//        }

//        public int Miscellaneous
//        {
//            get => _miscellaneous;
//            set => SetProperty(ref _miscellaneous, value);
//        }

//        public IEnumerable<ISeries> Series { get; set; }

//        public Chart()
//        {

//            var groceriesSeries = new PieSeries<int>
//            {
//                Values = new[] { Groceries },
//                Name = "Groceries",
//                InnerRadius = 50
//            };

//            var transportationSeries = new PieSeries<int>
//            {
//                Values = new[] { Transportation },
//                Name = "Transportation",
//                InnerRadius = 50
//            };

//            var housingSeries = new PieSeries<int>
//            {
//                Values = new[] { Housing },
//                Name = "Housing",
//                InnerRadius = 50
//            };

//            var utilitiesSeries = new PieSeries<int>
//            {
//                Values = new[] { Utilities },
//                Name = "Utilities",
//                InnerRadius = 50
//            };

//            var healthcareSeries = new PieSeries<int>
//            {
//                Values = new[] { Healthcare },
//                Name = "Healthcare",
//                InnerRadius = 50
//            };

//            var clothingSeries = new PieSeries<int>
//            {
//                Values = new[] { Clothing },
//                Name = "Clothing",
//                InnerRadius = 50
//            };

//            var entertainmentSeries = new PieSeries<int>
//            {
//                Values = new[] { Transportation },
//                Name = "Entertainment",
//                InnerRadius = 50
//            };

//            var miscellaneousSeries = new PieSeries<int>
//            {
//                Values = new[] { Miscellaneous },
//                Name = "Miscellaneous",
//                InnerRadius = 50
//            };

//            Series = new List<ISeries> { groceriesSeries, transportationSeries, housingSeries, utilitiesSeries, healthcareSeries, clothingSeries, entertainmentSeries, miscellaneousSeries };

//        }
//    }
//}

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinancialTracker
{
    public partial class Chart : ObservableObject
    {

        public Canvas ChartCanvas { get; set; }
        private ItemsControl detailsItemsControl { get; set; }
        private List<Category> Categories { get; set; }

        public Chart()
        {
            float pieWidth = 200, pieHeight = 200, centerX = pieWidth / 2, centerY = pieHeight / 2, radius = pieWidth / 2;

            detailsItemsControl = new ItemsControl();

            ChartCanvas = new Canvas();
            ChartCanvas.Width = pieWidth;
            ChartCanvas.Height = pieHeight;

            Categories = new List<Category>()
            {
                new Category
                {
                    Title = "Category#01",
                    Percentage = 20,
                    ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4472C4")),
                },

                new Category
                {
                    Title = "Category#02",
                    Percentage = 60,
                    ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED7D31")),
                },

                new Category
                {
                    Title = "Category#03",
                    Percentage = 5,
                    ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC000")),
                },

                new Category
                {
                    Title = "Category#04",
                    Percentage = 10,
                    ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5B9BD5")),
                },

                new Category
                {
                    Title = "Category#05",
                    Percentage = 5,
                    ColorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#A5A5A5")),
                }, 
            };

            detailsItemsControl.ItemsSource = Categories;

            float angle = 0, prevAngle = 0;
            foreach (var category in Categories)
            {
                double line1X = (radius * Math.Cos(angle * Math.PI / 180)) + centerX;
                double line1Y = (radius * Math.Sin(angle * Math.PI / 180)) + centerY;

                angle = category.Percentage * (float)360 / 100 + prevAngle;
                Debug.WriteLine(angle);

                double arcX = (radius * Math.Cos(angle * Math.PI / 180)) + centerX;
                double arcY = (radius * Math.Sin(angle * Math.PI / 180)) + centerY;

                var line1Segment = new LineSegment(new Point(line1X, line1Y), false);
                double arcWidth = radius, arcHeight = radius;
                bool isLargeArc = category.Percentage > 50;
                var arcSegment = new ArcSegment()
                {
                    Size = new Size(arcWidth, arcHeight),
                    Point = new Point(arcX, arcY),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = isLargeArc,
                };
                var line2Segment = new LineSegment(new Point(centerX, centerY), false);

                var pathFigure = new PathFigure(
                    new Point(centerX, centerY),
                    new List<PathSegment>()
                    {
                        line1Segment,
                        arcSegment,
                        line2Segment,
                    },
                    true);

                var pathFigures = new List<PathFigure>() { pathFigure, };
                var pathGeometry = new PathGeometry(pathFigures);
                var path = new Path()
                {
                    Fill = category.ColorBrush,
                    Data = pathGeometry,
                };
                ChartCanvas.Children.Add(path);

                prevAngle = angle;

                var outline1 = new Line()
                {
                    X1 = centerX,
                    Y1 = centerY,
                    X2 = line1Segment.Point.X,
                    Y2 = line1Segment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };
                var outline2 = new Line()
                {
                    X1 = centerX,
                    Y1 = centerY,
                    X2 = arcSegment.Point.X,
                    Y2 = arcSegment.Point.Y,
                    Stroke = Brushes.White,
                    StrokeThickness = 5,
                };

                ChartCanvas.Children.Add(outline1);
                ChartCanvas.Children.Add(outline2);
            }
        }
    }

    public class Category
    {
        public float Percentage { get; set; }
        public string Title { get; set; }
        public Brush ColorBrush { get; set; }
    }
}
