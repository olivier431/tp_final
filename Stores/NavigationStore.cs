using System;
using tp_final.ViewModels;

namespace tp_final.Stores
{
    public class NavigationStore
    {
        private BaseViewModel currentViewModel;
        public event Action CurrentViewModelChanged;

        public BaseViewModel CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}