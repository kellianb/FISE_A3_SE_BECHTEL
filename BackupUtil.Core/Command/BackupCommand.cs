using BackupUtil.Core.Executor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Command;

public class BackupCommand
{
    private readonly IBackupTransactionExecutor _receiver;
    private readonly BackupTransaction _transaction;

    internal BackupCommand(IBackupTransactionExecutor receiver, BackupTransaction transaction)
    {
        _receiver = receiver;
        _transaction = transaction;
    }

    public void Execute()
    {
        _receiver.Execute(_transaction);
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
