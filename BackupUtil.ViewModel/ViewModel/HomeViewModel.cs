using System.Windows.Input;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Service;

namespace BackupUtil.ViewModel.ViewModel;

public class HomeViewModel : ViewModelBase
{
    public HomeViewModel(JobListingViewModel jobListingViewModel, TransactionViewModel transactionViewModel,
        NavigationService<JobCreationViewModel> jobCreationNavigationService,
        NavigationService<SettingsViewModel> settingsNavigationService)
    {
        JobListingViewModel = jobListingViewModel;
        TransactionViewModel = transactionViewModel;

        OpenJobCreationCommand = new NavigateCommand<JobCreationViewModel>(jobCreationNavigationService);
        OpenSettingsCommand = new NavigateCommand<SettingsViewModel>(settingsNavigationService);
    }

    public JobListingViewModel JobListingViewModel { get; }
    public TransactionViewModel TransactionViewModel { get; }

    #region Commands

    // Opens the job creation view model
    public ICommand OpenJobCreationCommand { get; }

    // Opens the settings view model
    public ICommand OpenSettingsCommand { get; }

    #endregion
}
