using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class DisplayAllTransactionsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobManager _jobManager;

    public DisplayAllTransactionsCommand(JobListingViewModel jobListingViewModel, JobManager jobManager)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobManager = jobManager;
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
