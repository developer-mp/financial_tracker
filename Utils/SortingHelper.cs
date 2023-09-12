using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FinancialTracker
{
    public class SortingHelper
    {
        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private FrameworkElement _mainWindow;
        private ListView _listView;

        public SortingHelper(FrameworkElement mainWindow, ListView listView)
        {
            _mainWindow = mainWindow;
            _listView = listView;
        }

        public void SetDefaultSorting(string defaultSortColumn, string defaultSortDirection)
        {
            var gridView = _listView.View as GridView;

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
                        sortColumn.HeaderTemplate = _mainWindow.FindResource("HeaderArrowUp") as DataTemplate;
                    }
                    else
                    {
                        sortColumn.HeaderTemplate = _mainWindow.FindResource("HeaderArrowDown") as DataTemplate;
                    }

                    Sort(sortColumn.Header.ToString(), sortDirection);
                }
            }
        }

        public void ColumnHeaderClicked(GridViewColumnHeader headerClicked)
        {
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked == _lastHeaderClicked)
                    {
                        direction = _lastDirection == ListSortDirection.Ascending
                            ? ListSortDirection.Descending
                            : ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
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

                    ClearSorting();

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          _mainWindow.FindResource("HeaderArrowUp") as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          _mainWindow.FindResource("HeaderArrowDown") as DataTemplate;
                    }

                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        public void ClearSorting()
        {
            if (_listView.View is GridView gridView)
            {
                ICollectionView dataView = CollectionViewSource.GetDefaultView(_listView.ItemsSource);
                dataView.SortDescriptions.Clear();

                foreach (GridViewColumn column in gridView.Columns)
                {
                    column.HeaderTemplate = null;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(_listView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
    }
}
