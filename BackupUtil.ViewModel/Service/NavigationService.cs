using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Service;

    public class NavigationService<TViewModel>(Func<TViewModel> createViewModel)
        where TViewModel : ViewModelBase
    {
        private static NavigationStore navigationStore => NavigationStore.Instance;

        public void Navigate()
        {
            navigationStore.CurrentViewModel = createViewModel();
        }
    }
