using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using tp_final.Stores;
using tp_final.ViewModels;

namespace tp_final
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            NavigationStore navigationStore = new NavigationStore();

            navigationStore.CurrentViewModel = new WelcomeViewModel(navigationStore);

           MainWindow window = new MainWindow() {
                DataContext = new MainViewModel(navigationStore)
            };
            window.Show();
        }
    }
}
