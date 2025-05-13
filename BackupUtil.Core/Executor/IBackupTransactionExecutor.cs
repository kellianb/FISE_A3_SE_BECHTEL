using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

internal interface IBackupTransactionExecutor
{
    void Execute(BackupTransaction transaction);
}
