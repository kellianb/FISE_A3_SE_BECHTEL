using BackupUtil.Core.Command;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateAllTransactionsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;
    private readonly BackupCommandStore _backupCommandStore;

    public CreateAllTransactionsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore, BackupCommandStore backupCommandStore)
    {
        _jobListingViewModel = jobListingViewModel;
        _jobStore = jobStore;
        _backupCommandStore = backupCommandStore;
    }

    public override bool CanExecute(object? parameter)
    {
        return base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        BackupCommand backupCommand = _jobStore.RunAll();
        _backupCommandStore.BackupCommands.Add(backupCommand);
        _jobListingViewModel.GetTransactionsDetails(backupCommand);
    }

}
