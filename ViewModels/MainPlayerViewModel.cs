using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp_final.Commands;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    class MainPlayerViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;

        public MainPlayerViewModel(NavigationStore _navigationStore)
        {
            navigationStore = _navigationStore;
            
        }
    }
}
