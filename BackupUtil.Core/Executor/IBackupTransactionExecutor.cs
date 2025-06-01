
namespace BackupUtil.Core.Executor;

internal interface IBackupTransactionExecutor
{
    delegate void CancelCallback();
    delegate void ProgressCallback(string message, CurrentOperationType? operationType);

    void Execute();

    public Task ExecuteAsync(CancellationToken cancellationToken = default);
}
