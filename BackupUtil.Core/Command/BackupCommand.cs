using BackupUtil.Core.Executor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Command;

public class BackupCommand(IBackupTransactionExecutor receiver, BackupTransaction transaction)
{
    public void Execute()
    {
        receiver.Execute(transaction);
    }

    public long GetTotalCopiedFileSize()
    {
        return transaction.GetTotalCopiedFileSize();
    }

    public Dictionary<FileChangeType, string[]> GetConcernedFiles()
    {
        return transaction.FileChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }

    public Dictionary<DirectoryChangeType, string[]> GetConcernedDirectories()
    {
        return transaction.DirectoryChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }
}
