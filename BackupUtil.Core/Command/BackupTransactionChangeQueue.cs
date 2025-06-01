using System.Collections.Concurrent;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Transaction.FileMask;

namespace BackupUtil.Core.Command;

/// <summary>
///     Queue changes in a BackupTransaction
/// </summary>
internal class BackupTransactionChangeQueue
{
    public readonly ConcurrentQueue<DirectoryChange> DirectoryChanges = new();

    public readonly ConcurrentQueue<FileChange> FileChanges = new();
    public readonly ConcurrentQueue<FileChange> ParallelFileChanges = new();
    public readonly ConcurrentQueue<FileChange> PrioritizedFileChanges = new();
    public readonly ConcurrentQueue<FileChange> PrioritizedParallelFileChanges = new();

    internal BackupTransactionChangeQueue(BackupTransaction transaction)
    {
        SetInitialStats(transaction);

        foreach (DirectoryChange change in transaction.DirectoryChanges)
        {
            DirectoryChanges.Enqueue(change);
        }

        foreach (FileChange fileChange in transaction.FileChanges)
        {
            // Handle prioritized files
            if (fileChange.Effects.HasFlag(FileMaskEffect.Prioritize))
            {
                if (fileChange.Effects.HasFlag(FileMaskEffect.Parallelize))
                {
                    PrioritizedParallelFileChanges.Enqueue(fileChange);
                    continue;
                }

                PrioritizedFileChanges.Enqueue(fileChange);
                continue;
            }

            // Handle non-prioritized files
            if (fileChange.Effects.HasFlag(FileMaskEffect.Parallelize))
            {
                FileChanges.Enqueue(fileChange);
                continue;
            }

            ParallelFileChanges.Enqueue(fileChange);
        }
    }

    #region Statistics

    private void SetInitialStats(BackupTransaction transaction)
    {
        TotalFileSize = transaction.GetTotalCopiedFileSize();
        _remainingFileSize = TotalFileSize;
        TotalFileCount = transaction.FileChanges.Count;
        TotalDirectoryCount = transaction.DirectoryChanges.Count;
    }

    public void ReportFileProcessed(FileChange change)
    {
        Interlocked.Add(ref _remainingFileSize, -change.FileSize);
    }

    // Size of files that will be copied
    public long TotalFileSize { get; private set; }

    private long _remainingFileSize;

    public long RemainingFileSize => Interlocked.Read(ref _remainingFileSize);

    // Number of files which will be copied
    public long TotalFileCount { get; private set; }

    public long RemainingFileCount => PrioritizedFileChanges.Count
                                      + PrioritizedParallelFileChanges.Count
                                      + FileChanges.Count
                                      + ParallelFileChanges.Count;

    // Number of directories which will be copied
    public long TotalDirectoryCount { get; private set; }

    public long RemainingDirectoryCount => DirectoryChanges.Count;

    #endregion
}
