using BackupUtil.Core.Command;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateTransactionForAllJobsCommand : CommandBase
{
    private readonly BackupCommandStore _backupCommandStore;
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;

    public CreateTransactionForAllJobsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore,
        BackupCommandStore backupCommandStore)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobStore = jobStore;
        _backupCommandStore = backupCommandStore;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.Jobs.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        BackupCommand backupCommand = _jobStore.RunAll();
        _backupCommandStore.AddBackupCommand(backupCommand);
    }
}
