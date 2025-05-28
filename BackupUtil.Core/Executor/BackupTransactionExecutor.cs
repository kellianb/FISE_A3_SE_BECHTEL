using BackupUtil.Core.Command;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.I18n;
using Serilog;
using SerilogTimings.Extensions;

namespace BackupUtil.Core.Executor;

internal class BackupTransactionExecutor : IBackupTransactionExecutor
{
    public void Execute(BackupTransaction transaction)
    {
        ExecuteAsync(transaction).GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(BackupTransaction transaction,
        IProgress<BackupProgress>? progress = null,
        CancellationToken cancellationToken = default,
        ProgramFilter? programFilter = null)
    {
        using IDisposable _ = Log.Logger.TimeOperation("Executing transaction: {@BackupTransaction}", transaction);

        long totalOperationsSize = transaction.DirectoryChanges.Count
                                   + transaction.FileChanges.Sum(x => x.FileSize);
        long completedOperationsSize = 0;

        // TODO pop changes from lists when done processing them
        // to prevent duplicate changes when resuming a cancelled transaction
        // Process directory changes
        foreach (DirectoryChange change in transaction.DirectoryChanges)
        {
            programFilter?.CheckForBannedPrograms();
            cancellationToken.ThrowIfCancellationRequested();

            ExecuteDirectoryChange(change);

            Interlocked.Increment(ref completedOperationsSize);

            if (progress != null)
            {
                ReportProgress(progress, completedOperationsSize, totalOperationsSize, change.TargetPath,
                    CurrentOperationType.CreatingDirectories);
            }
        }

        // Process file changes
        foreach (FileChange change in transaction.FileChanges)
        {
            programFilter?.CheckForBannedPrograms();
            cancellationToken.ThrowIfCancellationRequested();

            await ExecuteFileChangeAsyncWithRetry(change, cancellationToken);

            Interlocked.Increment(ref completedOperationsSize);

            if (progress != null)
            {
                ReportProgress(progress, completedOperationsSize, totalOperationsSize, change.TargetPath,
                    CurrentOperationType.CopyingFiles);
            }
        }
    }

    #region Directory changes

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

    #endregion

    #region Report progress

    private static void ReportProgress(
        IProgress<BackupProgress> progress,
        long completedOperationsSize,
        long totalOperationsSize,
        string currentItem,
        CurrentOperationType currentOperationType)
    {
        BackupProgress progressInfo = new()
        {
            CompletedOperationsSize = completedOperationsSize,
            TotalOperationsSize = totalOperationsSize,
            PercentComplete = (int)((float)completedOperationsSize / totalOperationsSize * 100),
            CurrentItem = currentItem,
            CurrentOperationType = currentOperationType
        };

        progress.Report(progressInfo);
    }

    #endregion

    #region File changes

    private static async Task ExecuteFileChangeAsyncWithRetry(FileChange change, CancellationToken cancellationToken)
    {
        using IDisposable _ = change.Encryptor is null
            ? Log.Logger.TimeOperation("Copying file: {@string}", change.SourcePath ?? "")
            : Log.Logger.TimeOperation("Copying and encrypting file: {@string}", change.SourcePath ?? "");

        const int maxRetries = 3;
        const int delayMs = 500;

        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                await ExecuteFileChangeAsync(change, cancellationToken);
                return;
            }
            catch (IOException ex) when (retry < maxRetries - 1)
            {
                // Log the retry attempt
                Log.Warning("File access error. Retrying ({Attempt}/{MaxRetries}): {Message}",
                    retry + 1, maxRetries, ex.Message);

                // Wait before retrying
                await Task.Delay(delayMs * (retry + 1), cancellationToken);
            }
        }

        // If we get here, all retries failed - throw the exception
        throw new IOException($"Could not access file after {maxRetries} attempts: " +
                              (change.ChangeType == FileChangeType.Delete ? change.TargetPath : change.SourcePath));
    }

    private static async Task ExecuteFileChangeAsync(FileChange change, CancellationToken cancellationToken)
    {
        switch (change.ChangeType)
        {
            case FileChangeType.Create:
            case FileChangeType.Modify:
                // Open source and target files
                await using (FileStream sourceStream = new(change.SourcePath!, FileMode.Open, FileAccess.Read,
                                 FileShare.ReadWrite))
                await using (FileStream targetStream = new(change.TargetPath, FileMode.Create, FileAccess.Write,
                                 FileShare.ReadWrite))
                {
                    if (change.Encryptor == null)
                    {
                        await sourceStream.CopyToAsync(targetStream, 81920, cancellationToken);
                    }
                    else
                    {
                        using StreamReader reader = new(sourceStream);
                        string content = await reader.ReadToEndAsync(cancellationToken);

                        string encryptedContent = change.Encryptor.Encrypt(content);

                        await using StreamWriter writer = new(targetStream);
                        await writer.WriteAsync(encryptedContent);
                    }
                }

                break;
            case FileChangeType.Delete:
                File.Delete(change.TargetPath);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}

// Progress reporting class
public struct BackupProgress
{
    public long CompletedOperationsSize { get; set; }
    public long TotalOperationsSize { get; set; }
    public int PercentComplete { get; set; }
    public string CurrentItem { get; set; }
    public CurrentOperationType CurrentOperationType { get; set; }
}

public enum CurrentOperationType
{
    [I18NKey("executingDirectoryChanges")] CreatingDirectories,

    [I18NKey("executingPriorityFileChanges")]
    CopyingPriorityFiles,
    [I18NKey("executingFileChanges")] CopyingFiles
}
