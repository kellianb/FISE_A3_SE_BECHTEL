using System.Diagnostics;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using Serilog;
using Serilog.Context;
using SerilogTracing;

namespace BackupUtil.Core.Executor;

internal class BackupTransactionExecutor : IBackupTransactionExecutor
{
    public void Execute(BackupTransaction transaction)
    {
        using LoggerActivity activity = Log.Logger.StartActivity("Executing transaction: {@BackupTransaction}", transaction);

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
        Stopwatch stopwatch = Stopwatch.StartNew();

        switch (change.ChangeType)
        {
            case FileChangeType.Create:
                File.Copy(change.SourcePath!, change.TargetPath, true);
                break;
            case FileChangeType.Modify:
                File.Copy(change.SourcePath!, change.TargetPath, true);
                break;
            case FileChangeType.Delete:
                File.Delete(change.TargetPath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        stopwatch.Stop();
        change.ExecutionTime = stopwatch.ElapsedMilliseconds;
    }
}
