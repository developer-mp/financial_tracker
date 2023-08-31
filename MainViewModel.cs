using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace FinancialTracker
{
    public class MainViewModel : ObservableObject
    {
        private List<Category> _categories;
        public List<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }
    }
}
