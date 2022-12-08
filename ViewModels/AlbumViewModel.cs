using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using tp_final.Services;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    class AlbumViewModel : BaseViewModel
    {
        ICollectionView tuneViewSource;
        ICollectionView albumViewSource;
        TestDataServices testDataServices = new TestDataServices();
        private readonly NavigationStore navigationStore;

        public AlbumViewModel(NavigationStore _navigationStore) {
            TuneViewSource = CollectionViewSource.GetDefaultView(testDataServices.LesTunes);
            navigationStore = _navigationStore;
        }

        public ICollectionView TuneViewSource
        {
            get => tuneViewSource;
            set
            {
                tuneViewSource = value;
                OnPropertyChanged();
            }
        }
    }
}
