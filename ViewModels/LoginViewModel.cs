using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp_final.Commands;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;
        public DelegateCommand GoToRegisterCommand { get; set; }
        public LoginViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            GoToRegisterCommand = new DelegateCommand(GoToRegister);
        }

        public void GoToRegister()
        {
            navigationStore.CurrentViewModel = new RegisterViewModel(navigationStore);
        }
    }
}
