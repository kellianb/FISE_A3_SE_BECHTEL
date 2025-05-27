using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.I18n;
using Serilog;
using SerilogTimings.Extensions;

namespace BackupUtil.Core.Executor;

internal class BackupTransactionExecutor : IBackupTransactionExecutor
{
    // For backward compatibility
    public void Execute(BackupTransaction transaction)
    {
        ExecuteAsync(transaction).GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(BackupTransaction transaction,
        IProgress<BackupProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        using IDisposable _ = Log.Logger.TimeOperation("Executing transaction: {@BackupTransaction}", transaction);

        int totalOperations = transaction.DirectoryChanges.Count + transaction.FileChanges.Count;
        int completedOperations = 0;

        // Process directory changes
        foreach (DirectoryChange change in transaction.DirectoryChanges)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ExecuteDirectoryChange(change);

            Interlocked.Increment(ref completedOperations);

            if (progress != null)
            {
                ReportProgress(progress, completedOperations, totalOperations, change.TargetPath,
                    CurrentOperation.CreatingDirectories);
            }
        }

        // Process file changes
        foreach (FileChange change in transaction.FileChanges)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await ExecuteFileChangeAsyncWithRetry(change, cancellationToken);

            Interlocked.Increment(ref completedOperations);

            if (progress != null)
            {
                ReportProgress(progress, completedOperations, totalOperations, change.TargetPath,
                    CurrentOperation.CopyingFiles);
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

    #region Report progress

    private static void ReportProgress(IProgress<BackupProgress> progress, int completed, int total, string currentItem,
        CurrentOperation currentOperation)
    {
        BackupProgress progressInfo = new()
        {
            CompletedOperations = completed,
            TotalOperations = total,
            PercentComplete = (int)((float)completed / total * 100),
            CurrentItem = currentItem,
            CurrentOperation = currentOperation
        };

        progress.Report(progressInfo);
    }
}

// Progress reporting class
public struct BackupProgress
{
    public int CompletedOperations { get; set; }
    public int TotalOperations { get; set; }
    public int PercentComplete { get; set; }
    public string CurrentItem { get; set; }
    public CurrentOperation CurrentOperation { get; set; }
}

public enum CurrentOperation
{
    [I18NKey("executingDirectoryChanges")] CreatingDirectories,
    [I18NKey("executingPriorityFileChanges")] CopyingPriorityFiles,
    [I18NKey("executingFileChanges")] CopyingFiles
}

#endregion
