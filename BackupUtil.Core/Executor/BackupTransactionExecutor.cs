using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Executor;

public class BackupTransactionExecutor : IBackupTransactionExecutor
{
    public static void Execute(BackupTransaction transaction)
    {
        foreach (DirectoryChange change in transaction.DirectoryChanges)
        {
            ExecuteDirectoryChange(change);
        }

        foreach (FileChange change in transaction.FileChanges)
        {
            ExecuteFileChange(change);
        }
    }

    private static void ExecuteDirectoryChange(DirectoryChange change)
    {
        switch (change.ChangeType)
        {
            case DirectoryChangeType.Create:
                Directory.CreateDirectory(change.TargetPath);
                break;
            case DirectoryChangeType.Delete:
                Directory.Delete(change.TargetPath, true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ExecuteFileChange(FileChange change)
    {
        switch (change.ChangeType)
        {
            case FileChangeType.Create:
                File.Copy(change.SourcePath!, change.TargetPath);
                break;
            case FileChangeType.Modify:
                File.Delete(change.TargetPath);
                File.Copy(change.SourcePath!, change.TargetPath);
                break;
            case FileChangeType.Delete:
                File.Delete(change.TargetPath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
