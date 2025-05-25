using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class NavigateCommand : CommandBase
// where TViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public NavigateCommand(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
    }

    public override void Execute(object? parameter)
    {
        _navigationStore.CurrentViewModel = new JobCreationViewModel(new JobManager());
    }
}
