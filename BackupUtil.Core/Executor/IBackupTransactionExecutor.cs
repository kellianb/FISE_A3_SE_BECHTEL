using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

public interface IBackupTransactionExecutor
{
    void Execute(BackupTransaction transaction);
}
