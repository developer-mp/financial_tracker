using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Python.Runtime;

namespace FinancialTracker
{
    public class ChartGenerator
    {
        private readonly string _pythonDllPath;

        public ChartGenerator(string pythonDllPath)
        {
            _pythonDllPath = pythonDllPath;
        }

        public BitmapImage GenerateChart(List<ExpenseByCategory> expensesByCategory)
        {
            if (!string.IsNullOrEmpty(_pythonDllPath))
            {
                Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", _pythonDllPath);
                PythonEngine.Initialize();

                using (Py.GIL())
                {
                    dynamic np = Py.Import("numpy");
                    dynamic plt = Py.Import("matplotlib.pyplot");

                    List<string> labels = new List<string>();
                    List<double> sizes = new List<double>();
                    List<string> colors = new List<string> { "#449E48", "#06CCB0", "#FF817E", "#F58216", "#FED679", "#866FFD", "#3388FF", "#82D5F9" };

                    foreach (var expense in expensesByCategory)
                    {
                        labels.Add($"{expense.Category} ${expense.TotalAmount:N2}");
                        sizes.Add(expense.TotalAmount);
                    }

                    plt.figure().set_figwidth(9);

                    dynamic wedges;
                    plt.pie(sizes, colors: colors, startangle: 140);
                    dynamic result = plt.pie(sizes, colors: colors, autopct: "%1.0f%%", startangle: 140);
                    wedges = result[0];

                    plt.legend(wedges, labels, loc: "center left", bbox_to_anchor: new double[] { 1, 0.5 }, fontsize: 12);

                    dynamic io = Py.Import("io");
                    dynamic buf = io.BytesIO();
                    plt.savefig(buf, format: "png");
                    buf.seek(0);

                    byte[] chartBytes = buf.getvalue();

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(chartBytes);
                    bitmapImage.EndInit();

                    return bitmapImage;
                }
            }
            else
            {
                throw new InvalidOperationException("Python DLL path not configured");
            }
        }
    }
}