using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

    public class NavigateCommand<TViewModel>(NavigationService<TViewModel> navigationService) : CommandBase
        where TViewModel : ViewModelBase
    {
        public override void Execute(object? parameter)
        {
            navigationService.Navigate();
        }
    }
