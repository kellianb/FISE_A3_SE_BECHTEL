using BackupUtil.Core.Command;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateSelectedTransactionsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;
    private readonly BackupCommandStore _backupCommandStore;

    public CreateSelectedTransactionsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore, BackupCommandStore backupCommandStore)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobStore = jobStore;
        _backupCommandStore = backupCommandStore;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.SelectJobIndices.Any() && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        BackupCommand backupCommand = _jobStore.RunByIndices(_jobListingViewModel.SelectJobIndices);
        _backupCommandStore.BackupCommands.Add(backupCommand);
        _jobListingViewModel.GetTransactionsDetails(backupCommand);
    }
}
