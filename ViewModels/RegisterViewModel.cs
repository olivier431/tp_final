using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp_final.Commands;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;
        public DelegateCommand BackToLoginCommand { get; set; }

        public RegisterViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            BackToLoginCommand = new DelegateCommand(BackToLogin);
        }

        public void BackToLogin()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
        }
    }
}
