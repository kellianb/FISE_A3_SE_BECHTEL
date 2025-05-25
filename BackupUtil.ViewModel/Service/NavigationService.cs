using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Service;

public class NavigationService(NavigationStore navigationStore, Func<ViewModelBase> viewModelSource)
{
    public void Navigate()
    {
        navigationStore.CurrentViewModel = viewModelSource();
    }
}
