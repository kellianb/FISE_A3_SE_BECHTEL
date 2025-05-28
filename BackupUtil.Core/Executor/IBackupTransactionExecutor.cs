using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

internal interface IBackupTransactionExecutor
{
    void Execute(BackupTransaction transaction);

    public Task ExecuteAsync(BackupTransaction transaction,
        Action shouldCancel,
        Action<string, CurrentOperationType?> updateProgress,
        CancellationToken cancellationToken = default);
}
