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
    internal class AdminUserViewModel : BaseViewModel
    {
        ICollectionView userViewSource;
        TestDataServices testDataServices = new TestDataServices();
        private readonly NavigationStore navigationStore;
        public AdminUserViewModel(NavigationStore _navigationStore) {
            UserViewSource = CollectionViewSource.GetDefaultView(testDataServices.Lesusers);
            navigationStore = _navigationStore;
            
        }

        public ICollectionView UserViewSource
        {
            get => userViewSource;
            set
            {
                userViewSource = value;

            }
        }
    }


    
}
