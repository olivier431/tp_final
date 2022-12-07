using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;

        public RegisterViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
        }
    }
}
