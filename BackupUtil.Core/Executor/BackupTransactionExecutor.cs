using BackupUtil.Core.Encryptor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using Serilog;
using SerilogTimings.Extensions;

namespace BackupUtil.Core.Executor;

internal class BackupTransactionExecutor(IEncryptor encryptor) : IBackupTransactionExecutor
{
    public void Execute(BackupTransaction transaction)
    {
        using IDisposable _ = Log.Logger.TimeOperation("Executing transaction: {@BackupTransaction}", transaction);

        foreach (DirectoryChange change in transaction.DirectoryChanges)
        {
            ExecuteDirectoryChange(change);
        }

        foreach (FileChange change in transaction.FileChanges)
        {
            ExecuteFileChange(change);
        }
    }

    private void ExecuteDirectoryChange(DirectoryChange change)
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

    private void ExecuteFileChange(FileChange change)
    {
        switch (change.ChangeType)
        {
            case FileChangeType.Create:
            case FileChangeType.Modify:
                if (change.EncryptionKey == null)
                {
                    File.Copy(change.SourcePath!, change.TargetPath, true);
                }
                else
                {
                    File.WriteAllText(change.TargetPath,
                        encryptor.Encrypt(File.ReadAllText(change.SourcePath!), change.EncryptionKey));
                }

                break;
            case FileChangeType.Delete:
                File.Delete(change.TargetPath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
