using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

public interface IBackupTransactionExecutor
{
    static abstract void Execute(BackupTransaction transaction);
}
