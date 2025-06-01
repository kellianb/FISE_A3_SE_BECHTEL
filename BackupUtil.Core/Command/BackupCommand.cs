using BackupUtil.Core.Executor;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Command;

public sealed class BackupCommand : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Progress<BackupProgress> _progress;
    private readonly IProgress<BackupProgress> _progressReport;
    public readonly List<string> JobNames;

    private bool _disposed;


    internal BackupCommand(BackupTransaction transaction, List<string> jobNames)
    {
        JobNames = jobNames;
        _changeQueue = new BackupTransactionChangeQueue(transaction);
        _transaction = transaction;

        // Create initial cancellation token
        _cancellationTokenSource = new CancellationTokenSource();

        // Instantiate progress reporting types
        _progress = new Progress<BackupProgress>();
        _progressReport = _progress;
    }

    public event EventHandler<BackupProgress>? ProgressChanged
    {
        add => _progress.ProgressChanged += value;
        remove => _progress.ProgressChanged -= value;
    }

    #region BackupTransaction

    private readonly BackupTransaction _transaction;

    private readonly BackupTransactionChangeQueue _changeQueue;

    #endregion

    #region Program filter

    private readonly Lock _programFilterLock = new();

    private ProgramFilter? _programFilter;

    public ProgramFilter? ProgramFilter
    {
        get
        {
            lock (_programFilterLock)
            {
                return _programFilter;
            }
        }
        set
        {
            lock (_programFilterLock)
            {
                _programFilter = value;
            }
        }
    }

    #endregion

    #region Command state

    private readonly Lock _stateLock = new();

    private BackupCommandState _state = BackupCommandState.NotStarted;

    public BackupCommandState State
    {
        get
        {
            lock (_stateLock)
            {
                return _state;
            }
        }
        private set
        {
            lock (_stateLock)
            {
                _state = value;
            }
        }
    }

    #endregion

    #region Control ongoing backup

    public void Start()
    {
        // If the command is already running, finished or paused because of a banned program, do nothing
        if (State is BackupCommandState.Running
            or BackupCommandState.Finished
            or BackupCommandState.Stopped
            or BackupCommandState.PausedBannedProgram)
        {
            return;
        }

        // If the Command is not running or paused, execute it
        Task.Run(ExecuteAsync);
    }

    public void Pause()
    {
        State = BackupCommandState.Paused;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        State = BackupCommandState.Stopped;
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

    #region Run backup

    /// <summary>
    ///     Execute this backup without cancellation, pausing or progress reporting
    /// </summary>
    public void Execute()
    {
        BackupTransactionExecutor executor = new(_changeQueue, () => { }, (_, _) => { });

        executor.Execute();
    }


    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    ///     Execute this backup asynchronously
    /// </summary>
    /// <returns></returns>
    private async Task ExecuteAsync()
    {
        // There should never be two executors running at the same time for the same BackupCommand
        if (!await _semaphore.WaitAsync(0))
        {
            return;
        }

        State = BackupCommandState.Running;

        try
        {
            BackupTransactionExecutor executor = new(_changeQueue, ShouldCancel, UpdateProgress);

            await executor.ExecuteAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            UpdateProgress("");
            return;
        }
        catch (BannedProgramRunningException)
        {
            State = BackupCommandState.PausedBannedProgram;
            UpdateProgress("");
            return;
        }
        catch (Exception e)
        {
            State = BackupCommandState.PausedError;
            UpdateProgress("");
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        State = RemainingDirectoryCount + RemainingFileCount == 0
            ? BackupCommandState.Finished
            : BackupCommandState.Paused;
        UpdateProgress("");
    }

    private void ShouldCancel()
    {
        if (State == BackupCommandState.Paused)
        {
            throw new OperationCanceledException();
        }

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
            TotalDirectoryCount = TotalDirectoryCount,
            CompletedDirectoryCount = TotalDirectoryCount - RemainingDirectoryCount,
            TotalFileCount = TotalFileCount,
            CompletedFileCount = TotalFileCount - RemainingFileCount,
            CompletedPercentage = TotalFileSize > 0 ? 100 * (TotalFileSize - RemainingFileSize) / TotalFileSize : 0,
            CurrentItem = currentItem
        };

        _progressReport.Report(progress);
    }

    #endregion

    #region Statistics

    // Size of files that will be copied
    public long TotalFileSize => _changeQueue.TotalFileSize;

    public long RemainingFileSize => _changeQueue.RemainingFileSize;

    // Number of files which will be copied
    public long TotalFileCount => _changeQueue.TotalFileCount;

    public long RemainingFileCount => _changeQueue.RemainingFileCount;

    // Number of directories which will be copied
    public long TotalDirectoryCount => _changeQueue.TotalDirectoryCount;

    public long RemainingDirectoryCount => _changeQueue.RemainingDirectoryCount;

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
    /// <summary>
    ///     The Backup command has not been started
    /// </summary>
    NotStarted,

    /// <summary>
    ///     The Backup command is running
    /// </summary>
    Running,

    /// <summary>
    ///     The Backup command was manually paused
    /// </summary>
    Paused,

    /// <summary>
    ///     The Backup command was paused because a banned program is running
    /// </summary>
    PausedBannedProgram,

    /// <summary>
    ///     The Backup command was paused because it encountered an error while executing
    /// </summary>
    PausedError,

    /// <summary>
    ///     The Backup command was manually stopped
    /// </summary>
    Stopped,

    /// <summary>
    ///     The Backup command has run to completion
    /// </summary>
    Finished
}

public struct BackupProgress
{
    public BackupCommandState State { get; init; }
    public CurrentOperationType? Type { get; init; }

    // File size
    public long TotalFileSize { get; init; }
    public long CompletedFileSize { get; init; }

    // Directory count
    public long TotalDirectoryCount { get; init; }
    public long CompletedDirectoryCount { get; init; }

    // File count
    public long TotalFileCount { get; init; }
    public long CompletedFileCount { get; init; }

    // Completion percentage
    public long CompletedPercentage { get; init; }

    // Current item
    public string CurrentItem { get; init; }
}
