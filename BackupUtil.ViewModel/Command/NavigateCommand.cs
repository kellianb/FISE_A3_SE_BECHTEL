using BackupUtil.ViewModel.Service;

namespace BackupUtil.ViewModel.Command;

public class NavigateCommand(NavigationService navigationService) : CommandBase
{
    public override void Execute(object? parameter)
    {
        navigationService.Navigate();
    }
}
