using System.Windows;
using System.Windows.Controls;

namespace FinancialTracker
{
    public class ListViewSizeChanger
    {
        public static void ListViewSizeChanged(ListView listView)
        {
            GridView gridView = listView.View as GridView;

            if (gridView == null || gridView.Columns.Count == 0)
            {
                return;
            }

            double columnWidth = listView.ActualWidth / gridView.Columns.Count;

            foreach (GridViewColumn column in gridView.Columns)
            {
                column.Width = columnWidth;
            }
        }
    }
}
