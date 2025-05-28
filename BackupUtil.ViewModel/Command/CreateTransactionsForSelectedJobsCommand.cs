using BackupUtil.Core.Command;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateTransactionsForSelectedJobsCommand : CommandBase
{
    private readonly BackupCommandStore _backupCommandStore;
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;

    public CreateTransactionsForSelectedJobsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore,
        BackupCommandStore backupCommandStore)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobStore = jobStore;
        _backupCommandStore = backupCommandStore;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.SelectedJobIndices.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        BackupCommand backupCommand = _jobStore.RunByIndices(_jobListingViewModel.SelectedJobIndices);
        _backupCommandStore.AddBackupCommand(backupCommand);
    }
}
