using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Service;

public class NavigationService<TViewModel>(NavigationStore navigationStore, Func<TViewModel> viewModelSource)
    where TViewModel : ViewModelBase
{
    public void Navigate()
    {
        navigationStore.CurrentViewModel = viewModelSource();
    }
}
