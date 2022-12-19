using tp_final.Stores;

namespace tp_final.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NavigationStore navigationStore;

        public BaseViewModel CurrentViewModel => navigationStore.CurrentViewModel;
        public MainViewModel(NavigationStore _navigationStore)
        {
            navigationStore = _navigationStore;
            navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}