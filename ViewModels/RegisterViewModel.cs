using System.Windows;
using tp_final.Commands;
using tp_final.Models;
using tp_final.Stores;

namespace tp_final.ViewModels
{
    internal class RegisterViewModel : BaseViewModel
    {
        private string username;
        private string password;
        private string email;
        private readonly NavigationStore navigationStore;
        public DelegateCommand BackToLoginCommand { get; set; }
        public DelegateCommand RegisterCommand { get; set; }

        public RegisterViewModel(NavigationStore _navigationStore) {
            navigationStore = _navigationStore;
            BackToLoginCommand = new DelegateCommand(BackToLogin);
            RegisterCommand = new DelegateCommand(Register);
        }

        public string Username
        {
            get => username;
            set => username = value;
        }

        public string Password
        {
            get => password;
            set => password = value;
        }

        public string Email
        {
            get => email;
            set => email = value;
        }

        public void BackToLogin()
        {
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
        }

        public async void Register()
        {
            if (Username != null && Password != null && Email != null)
            {
                User user = await User.AddUserAsync(Username, Password, Email);
                Application.Current.Properties["CurrentUser"] = user;
                navigationStore.CurrentViewModel = new MainPlayerViewModel(navigationStore);
            }
            else
            {
                MessageBox.Show("a box is empty");
                return;
            }
            
        }
    }
}
