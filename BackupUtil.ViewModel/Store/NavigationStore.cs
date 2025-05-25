using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Store;

public class NavigationStore
{
    // private static readonly Lazy<NavigationStore> s_instance
    //     = new(() => new NavigationStore());

    private ViewModelBase? _currentViewModel;

    // private NavigationStore() { }

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel?.Dispose();
            _currentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    // public static NavigationStore Instance => s_instance.Value;

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}
