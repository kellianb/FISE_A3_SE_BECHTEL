using BackupUtil.Core.Executor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Command;

// TODO support pausing, and stopping if banned programs are running
public class BackupCommand
{
    private readonly IBackupTransactionExecutor _receiver;
    private readonly BackupTransaction _transaction;
    private ProgramFilter? _programFilter;
    public readonly List<string> JobNames;

    internal BackupCommand(IBackupTransactionExecutor receiver, BackupTransaction transaction, List<string> jobNames,
        ProgramFilter? programFilter = null)
    {
        _receiver = receiver;
        _transaction = transaction;
        JobNames = jobNames;
        _programFilter = programFilter;
    }

    public BackupCommandState State { get; private set; } = BackupCommandState.NotStarted;

    public void SetProgramFilter(ProgramFilter? programFilter)
    {
        _programFilter = programFilter;
    }

    public void Execute()
    {
        _programFilter?.CheckForBannedPrograms();

        _receiver.Execute(_transaction);
    }

    public Task ExecuteAsync(
        IProgress<BackupProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        switch (State)
        {
            case BackupCommandState.Running:
            case BackupCommandState.Finished:
                return Task.CompletedTask;
            case BackupCommandState.NotStarted:
            case BackupCommandState.Paused:
                State = BackupCommandState.Running;

                try
                {
                    _receiver.ExecuteAsync(_transaction, progress, cancellationToken, _programFilter);
                }
                catch
                {
                    // TODO check if there are any changes left
                    State = BackupCommandState.Paused;
                    throw;
                }

                break;
        }

        State = BackupCommandState.Finished;
        return Task.CompletedTask;
    }

    public long GetTotalCopiedFileSize()
    {
        return _transaction.GetTotalCopiedFileSize();
    }

    public Dictionary<FileChangeType, string[]> GetConcernedFiles()
    {
        return _transaction.FileChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }

    public Dictionary<DirectoryChangeType, string[]> GetConcernedDirectories()
    {
        return _transaction.DirectoryChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }
}

public enum BackupCommandState
{
    NotStarted,
    Running,
    Paused,
    Finished
}
