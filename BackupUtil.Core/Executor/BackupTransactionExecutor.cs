using System.Collections.Concurrent;
using BackupUtil.Core.Command;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Util;
using BackupUtil.I18n;
using SerilogTimings.Extensions;

namespace BackupUtil.Core.Executor;

internal class BackupTransactionExecutor(
    BackupTransactionChangeQueue changes,
    IBackupTransactionExecutor.CancelCallback? shouldCancel,
    IBackupTransactionExecutor.ProgressCallback? updateProgress)
    : IBackupTransactionExecutor
{
    private readonly IBackupTransactionExecutor.CancelCallback _shouldCancel = shouldCancel ?? (() => { });
    private readonly IBackupTransactionExecutor.ProgressCallback _updateProgress = updateProgress ?? ((_, _) => { });

    public void Execute()
    {
        ExecuteAsync().GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using IDisposable _ = Logging.StatusLog.Value.TimeOperation("Executing transaction");

        _shouldCancel();

        // Process directory changes
        while (changes.DirectoryChanges.TryDequeue(out DirectoryChange? change))
        {
            ExecuteDirectoryChange(change);

            _updateProgress(change.TargetPath, CurrentOperationType.CreatingDirectories);
            _shouldCancel();
        }

        // Process prioritized file changes
        await ProcessFileChangesSequentially(changes.PrioritizedFileChanges, CurrentOperationType.CopyingPriorityFiles,
            cancellationToken);

        // Process prioritized parallel file changes
        await ProcessFileChangesConcurrently(changes.PrioritizedParallelFileChanges,
            CurrentOperationType.CopyingPriorityFiles, cancellationToken);

        // Process file changes
        await ProcessFileChangesSequentially(changes.FileChanges, CurrentOperationType.CopyingFiles, cancellationToken);

        // Process parallel file changes
        await ProcessFileChangesConcurrently(changes.ParallelFileChanges, CurrentOperationType.CopyingFiles,
            cancellationToken);
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


    #region Process lists of file changes

    private async Task ProcessFileChangesSequentially(ConcurrentQueue<FileChange> fileChanges,
        CurrentOperationType type, CancellationToken cancellationToken)
    {
        while (fileChanges.TryDequeue(out FileChange? change))
        {
            await ExecuteFileChangeAsyncWithRetry(change, cancellationToken);

            changes.ReportFileProcessed(change);

            _updateProgress(change.TargetPath, type);
            _shouldCancel();
        }
    }

    private async Task ProcessFileChangesConcurrently(ConcurrentQueue<FileChange> fileChanges,
        CurrentOperationType type, CancellationToken cancellationToken)
    {
        List<Task> processingTasks = [];
        const int maxParallelTasks = 4;

        while (!fileChanges.IsEmpty || processingTasks.Count > 0)
        {
            // Start new tasks if we have capacity and items to process
            while (processingTasks.Count < maxParallelTasks && fileChanges.TryDequeue(out FileChange? change))
            {
                Task task = ProcessSingleFileChangeAsync(change, type, cancellationToken);
                processingTasks.Add(task);
            }

            if (processingTasks.Count <= 0)
            {
                continue;
            }

            Task completedTask = await Task.WhenAny(processingTasks);
            await completedTask;
            processingTasks.Remove(completedTask);
            _shouldCancel();
        }
    }

    private async Task ProcessSingleFileChangeAsync(FileChange change, CurrentOperationType type,
        CancellationToken cancellationToken)
    {
        await ExecuteFileChangeAsyncWithRetry(change, cancellationToken);

        changes.ReportFileProcessed(change);

        _updateProgress(change.TargetPath, type);
    }

    #endregion

    #region Execute file changes

    private static async Task ExecuteFileChangeAsyncWithRetry(FileChange change, CancellationToken cancellationToken)
    {
        using IDisposable _ = change.Encryptor is null
            ? Logging.DailyLog.Value.TimeOperation("Copying file: {@string}", change.SourcePath ?? "")
            : Logging.DailyLog.Value.TimeOperation("Copying and encrypting file: {@string}", change.SourcePath ?? "");

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
                Logging.DailyLog.Value.Warning("File access error. Retrying ({Attempt}/{MaxRetries}): {Message}",
                    retry + 1, maxRetries, ex.Message);

                // Wait before retrying
                await Task.Delay(delayMs * (retry + 1), cancellationToken);
            }
        }

        // If we get here, all retries failed - log and throw an exception
        string? filePath = change.ChangeType == FileChangeType.Delete ? change.TargetPath : change.SourcePath ?? "";

        Logging.DailyLog.Value.Warning("Failed to access file after {@int} attempts: {@string}", maxRetries, filePath);

        throw new IOException($"Could not access file after {maxRetries} attempts: {filePath}");
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

public enum CurrentOperationType
{
    [I18NKey("executingDirectoryChanges")] CreatingDirectories,

    [I18NKey("executingPriorityFileChanges")]
    CopyingPriorityFiles,
    [I18NKey("executingFileChanges")] CopyingFiles
}
