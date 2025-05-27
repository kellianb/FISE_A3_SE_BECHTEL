using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class MainViewModel : ViewModelBase
{
    private readonly NavigationStore _navigationStore;

    public MainViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;

        _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }

    public ViewModelBase? CurrentViewModel => _navigationStore.CurrentViewModel;

    public override void Dispose()
    {
        _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
        base.Dispose();
    }

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}
