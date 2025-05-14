namespace BackupUtil.ViewModel;

public class MainViewModel : ViewModelBase
{
    public ViewModelBase CurrentViewModel { get; }

    public MainViewModel()
    {
        CurrentViewModel = new JobListViewModel();
    }
}
