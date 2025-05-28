using BackupUtil.Core.Command;
using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Executor;

internal interface IBackupTransactionExecutor
{
    void Execute(BackupTransaction transaction);

    public Task ExecuteAsync(BackupTransaction transaction,
        IProgress<BackupProgress>? progress = null,
        CancellationToken cancellationToken = default,
        ProgramFilter? programFilter = null);
}
