using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

internal interface IBackupTransactionExecutor
{
    delegate void CancelCallback();
    delegate void ProgressCallback(string message, CurrentOperationType? operationType);

    void Execute(BackupTransaction transaction);

    public Task ExecuteAsync(BackupTransaction transaction,
        CancelCallback shouldCancel,
        ProgressCallback updateProgress,
        CancellationToken cancellationToken = default);
}
