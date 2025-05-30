using BackupUtil.Core.Executor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Command;

public sealed class BackupCommand : IDisposable
{
    private readonly IBackupTransactionExecutor _executor;
    private readonly Progress<BackupProgress> _progress;
    private readonly BackupTransaction _transaction;
    public readonly List<string> JobNames;
    private readonly IProgress<BackupProgress> progressReport;

    private CancellationTokenSource _cancellationTokenSource;

    private bool _disposed;
    private ProgramFilter? _programFilter;


    internal BackupCommand(IBackupTransactionExecutor executor, BackupTransaction transaction, List<string> jobNames,
        ProgramFilter? programFilter = null)
    {
        _executor = executor;
        _transaction = transaction;
        JobNames = jobNames;
        _programFilter = programFilter;

        // Create initial cancellation token
        _cancellationTokenSource = new CancellationTokenSource();

        // Set totals
        TotalFileSize = RemainingFileSize;
        TotalFileCount = RemainingFileCount;
        TotalDirectoriesCount = RemainingDirectoriesCount;

        // Instantiate progress reporting types
        _progress = new Progress<BackupProgress>();
        progressReport = _progress;
    }

    public BackupCommandState State { get; private set; } = BackupCommandState.NotStarted;

    public event EventHandler<BackupProgress>? ProgressChanged
    {
        add => _progress.ProgressChanged += value;
        remove => _progress.ProgressChanged -= value;
    }

    public void SetProgramFilter(ProgramFilter? programFilter)
    {
        _programFilter = programFilter;
    }

    #region Control ongoing backup

    public void Start()
    {
        // If the command is already running, finished or paused because of a banned program, do nothing
        if (State is BackupCommandState.Running
            or BackupCommandState.Finished
            or BackupCommandState.PausedBannedProgram)
        {
            return;
        }

        // If the Command is not running or paused, execute it
        Task.Run(ExecuteAsync);
    }

    public void Pause()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        State = BackupCommandState.Paused;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _cancellationTokenSource.Dispose();
            _semaphore?.Dispose();
        }

        _disposed = true;
    }

    #endregion

    #region Start backup

    /// <summary>
    ///     Execute this backup synchronously
    /// </summary>
    public void Execute()
    {
        _executor.Execute(_transaction);
    }


    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    ///     Execute this backup asynchronously
    /// </summary>
    /// <returns></returns>
    private async Task ExecuteAsync()
    {
        if (!await _semaphore.WaitAsync(0))
            // There should never be two executors running at the same time
            return;

        State = BackupCommandState.Running;

        try
        {
            await _executor.ExecuteAsync(_transaction, ShouldCancel, UpdateProgress, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            State = BackupCommandState.Paused;
            UpdateProgress("");
            return;
        }
        catch (BannedProgramRunningException)
        {
            State = BackupCommandState.PausedBannedProgram;
            UpdateProgress("");
            return;
        }
        finally
        {
            _semaphore.Release();
        }

        State = _transaction.DirectoryChanges.Count + _transaction.FileChanges.Count == 0
            ? BackupCommandState.Finished
            : BackupCommandState.Paused;
        UpdateProgress("");
    }

    private void ShouldCancel()
    {
        _cancellationTokenSource.Token.ThrowIfCancellationRequested();

        _programFilter?.ThrowIfBannedProgramDetected();
    }

    private void UpdateProgress(
        string currentItem,
        CurrentOperationType? currentOperationType = null
    )
    {
        BackupProgress progress = new()
        {
            State = State,
            Type = currentOperationType,
            TotalFileSize = TotalFileSize,
            CompletedFileSize = TotalFileSize - RemainingFileSize,
            PercentComplete = TotalFileSize > 0 ? 100 * (TotalFileSize - RemainingFileSize) / TotalFileSize : 0,
            CurrentItem = currentItem
        };

        progressReport.Report(progress);
    }

    #endregion

    #region Statistics

    // Size of files that will be copied
    public long TotalFileSize { get; }

    public long RemainingFileSize => _transaction.GetTotalCopiedFileSize();

    // Number of files which will be copied
    public long TotalFileCount { get; }

    public long RemainingFileCount => _transaction.FileChanges.Count;

    // Number of directories which will be copied
    public long TotalDirectoriesCount { get; }

    public long RemainingDirectoriesCount => _transaction.DirectoryChanges.Count;

    #endregion

    #region List concerned files and directories

    public Dictionary<FileChangeType, string[]> GetConcernedFiles()
    {
        return _transaction.FileChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }

    public Dictionary<DirectoryChangeType, string[]> GetConcernedDirectories()
    {
        return _transaction.DirectoryChanges
            .GroupBy(change => change.ChangeType)
            .ToDictionary(
                group => group.Key,
                group => group.Select(change => change.TargetPath).ToArray()
            );
    }

    #endregion
}

public enum BackupCommandState
{
    NotStarted,
    Running,
    Paused,
    PausedBannedProgram,
    Finished
}

public struct BackupProgress
{
    public BackupCommandState State { get; set; }
    public CurrentOperationType? Type { get; set; }
    public long TotalFileSize { get; set; }
    public long CompletedFileSize { get; set; }
    public long PercentComplete { get; set; }

    public string CurrentItem { get; set; }
}
