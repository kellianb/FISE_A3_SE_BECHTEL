using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class DisplayAllTransactionsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;

    public DisplayAllTransactionsCommand(JobListingViewModel jobListingViewModel)
    {
        _jobListingViewModel = jobListingViewModel;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _jobListingViewModel.GetAllTransactionsDetails();
    }

}
