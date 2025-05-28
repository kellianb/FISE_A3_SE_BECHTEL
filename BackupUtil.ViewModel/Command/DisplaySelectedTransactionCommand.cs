using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class DisplaySelectedTransactionCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobManager _jobManager;

    public DisplaySelectedTransactionCommand(JobListingViewModel jobListingViewModel, JobManager jobManager)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobManager = jobManager;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.isSelectedRow && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _jobListingViewModel.JobItems = parameter[0];
        _jobListingViewModel.SelectedJobItems = parameter[1];
        _jobListingViewModel.GetSelectedTransactionDetails();
    }
}
