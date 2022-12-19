using System.Windows;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class LoginViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;
        private string username;
        private string password;
        
        public DelegateCommand GoToRegisterCommand { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public LoginViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            GoToRegisterCommand = new DelegateCommand(GoToRegister);
            LoginCommand = new DelegateCommand(Login);
            
        }

        public string Username { 
            get => username; 
            set => username = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public void GoToRegister()
        {
            navigationStore.CurrentViewModel = new RegisterViewModel(navigationStore);
        }

        public async void Login()
        {
            if ((Username != "") && (Password != "")) {

                User user = await User.GetUserAsync(Username, Password);

                if (user == null)
                {
                    MessageBox.Show("Wrong username or password !");
                    return;
                }

                Application.Current.Properties["CurrentUser"] = user;
                navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
            }
            else
            {
                MessageBox.Show("Wrong username or password !");
                return;
            }
        }
    }
}
