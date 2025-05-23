namespace BackupUtil.ViewModel;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        CurrentViewModel = new JobListViewModel();
    }

    public ViewModelBase CurrentViewModel { get; }
}
