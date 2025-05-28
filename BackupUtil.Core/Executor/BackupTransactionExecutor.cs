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
        ExecuteAsync(transaction,
                () => { },
                (_, _) => { })
            .GetAwaiter().GetResult();
    }

    public async Task ExecuteAsync(BackupTransaction transaction,
        Action shouldCancel,
        Action<string, CurrentOperationType?> updateProgress,
        CancellationToken cancellationToken = default)
    {
        using IDisposable _ = Log.Logger.TimeOperation("Executing transaction: {@BackupTransaction}", transaction);

        // Process directory changes
        while (transaction.DirectoryChanges.Count > 0)
        {
            shouldCancel();
            DirectoryChange change = transaction.DirectoryChanges[0];

            ExecuteDirectoryChange(change);

            transaction.DirectoryChanges.RemoveAt(0);

            updateProgress(change.TargetPath, CurrentOperationType.CreatingDirectories);
        }

        // Process file changes
        while (transaction.FileChanges.Count > 0)
        {
            shouldCancel();
            FileChange change = transaction.FileChanges[0];

            await ExecuteFileChangeAsyncWithRetry(change, cancellationToken);

            transaction.FileChanges.RemoveAt(0);

            updateProgress(change.TargetPath, CurrentOperationType.CopyingFiles);
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
}

public enum CurrentOperationType
{
    [I18NKey("executingDirectoryChanges")] CreatingDirectories,
    [I18NKey("executingPriorityFileChanges")] CopyingPriorityFiles,
    [I18NKey("executingFileChanges")] CopyingFiles
}
