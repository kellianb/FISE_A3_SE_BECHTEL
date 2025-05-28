using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class DisplaySelectedTransactionCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;

    public DisplaySelectedTransactionCommand(JobListingViewModel jobListingViewModel)
    {
        _jobListingViewModel = jobListingViewModel;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.SelectJobIndices.Any() && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _jobListingViewModel.GetSelectedTransactionDetails();
    }
}
