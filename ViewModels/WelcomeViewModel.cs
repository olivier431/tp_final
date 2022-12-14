using tp_final.Commands;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class WelcomeViewModel : BaseViewModel
    {
        public DelegateCommand GoToLoginCommand { get; set; }
        private readonly NavigationStore navigationStore;
        public WelcomeViewModel(NavigationStore _navigationStore)
        {
            navigationStore = _navigationStore;
            GoToLoginCommand = new DelegateCommand(GoToLogin);
        }

        public void GoToLogin() {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
        }
    }
}
