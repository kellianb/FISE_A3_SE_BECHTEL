namespace BackupUtil.ViewModel.ViewModel;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        CurrentViewModel = new JobListingViewModel();
    }

    public ViewModelBase CurrentViewModel { get; }
}
