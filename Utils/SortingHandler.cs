using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FinancialTracker
{
    public class SortingHandler
    {
        private GridViewColumnHeader lastHeaderClicked = null;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;
        public void SetDefaultSorting(ListView listView, FrameworkElement mainWindow, string defaultSortColumn, string defaultSortDirection)
        {
            var gridView = listView.View as GridView;

            if (gridView != null)
            {
                var sortColumn = gridView.Columns.FirstOrDefault(column => column.Header.ToString() == defaultSortColumn);

                if (sortColumn != null)
                {
                    ListSortDirection sortDirection = defaultSortDirection == "Ascending"
                        ? ListSortDirection.Ascending
                        : ListSortDirection.Descending;

                    if (sortDirection == ListSortDirection.Ascending)
                    {
                        sortColumn.HeaderTemplate = mainWindow.FindResource("HeaderArrowUp") as DataTemplate;
                    }
                    else
                    {
                        sortColumn.HeaderTemplate = mainWindow.FindResource("HeaderArrowDown") as DataTemplate;
                    }

                    Sort(listView, sortColumn.Header.ToString(), sortDirection);
                    lastDirection = sortDirection;
                }
            }
        }

        public void ColumnHeaderClicked(ListView listView, FrameworkElement mainWindow, GridViewColumnHeader headerClicked)
        {
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked == lastHeaderClicked)
                    {
                        direction = lastDirection == ListSortDirection.Ascending
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    ClearSorting(listView);

                    Sort(listView, sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          mainWindow.FindResource("HeaderArrowUp") as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          mainWindow.FindResource("HeaderArrowDown") as DataTemplate;
                    }

                    if (lastHeaderClicked != null && lastHeaderClicked != headerClicked)
                    {
                        lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    lastHeaderClicked = headerClicked;
                    lastDirection = direction;
                }
            }
        }

        public void ClearSorting(ListView listView)
        {
            if (listView.View is GridView gridView)
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
                dataView.SortDescriptions.Clear();

                foreach (GridViewColumn column in gridView.Columns)
                {
                    column.HeaderTemplate = null;
                }
            }
        }

        private void Sort(ListView listView, string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
