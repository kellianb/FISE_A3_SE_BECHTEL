using System.Windows.Input;
using BackupUtil.Core.Command;
using BackupUtil.Core.Executor;
using BackupUtil.ViewModel.Command.Transaction;

namespace BackupUtil.ViewModel.ViewModel;

public class TransactionViewModel : ViewModelBase
{
    private readonly BackupCommand _command;

    /// <summary>
    ///     ViewModel for individual BackupCommand objects
    /// </summary>
    /// <param name="command">BackupCommand object</param>
    /// <param name="run">Callback to run the command</param>
    /// <param name="pause">Callback to pause the command</param>
    public TransactionViewModel(BackupCommand command, Action run, Action pause)
    {
        _command = command;
        JobNames = string.Join(", ", command.JobNames);

        // File size
        TotalFileSize = command.TotalFileSize;
        CompletedFileSize = command.TotalFileSize - command.RemainingFileSize;

        // Directory count
        TotalDirectoryCount = command.TotalDirectoryCount;
        CompletedDirectoryCount = command.TotalDirectoryCount - command.RemainingDirectoryCount;

        // File count
        TotalFileCount = command.TotalFileCount;
        CompletedFileCount = command.TotalFileCount - command.RemainingFileCount;

        // Listen to progress changes
        command.ProgressChanged += OnBackupCommandProgressChanged;

        RunTransactionCommand = new RunTransactionCommand(this, run);
        PauseTransactionCommand = new PauseTransactionCommand(this, pause);
    }

    public override void Dispose()
    {
        _command.ProgressChanged -= OnBackupCommandProgressChanged;
        base.Dispose();
    }

    #region Handle BackupCommand events

    private void OnBackupCommandProgressChanged(object? sender, BackupProgress e)
    {
        CompletedFileSize = e.CompletedFileSize;
        CompletedDirectoryCount = e.CompletedDirectoryCount;
        CompletedFileCount = e.CompletedFileCount;
        CompletedPercentage = e.CompletedPercentage;
        CurrentOperationType = e.Type;
        CurrentItem = e.CurrentItem;
        State = e.State;
    }

    #endregion

    #region Commands

    public ICommand RunTransactionCommand { get; }

    public ICommand PauseTransactionCommand { get; }

    #endregion

    #region IsSelected

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region JobNames

    private string _jobNames = "";

    public string JobNames
    {
        get => _jobNames;
        private set
        {
            _jobNames = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region FileSize

    private long _totalFileSize;

    public long TotalFileSize
    {
        get => _totalFileSize;
        private set
        {
            _totalFileSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FileSizeProgress));
        }
    }

    private long _completedFileSize;

    public long CompletedFileSize
    {
        get => _completedFileSize;
        private set
        {
            _completedFileSize = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FileSizeProgress));
        }
    }

    public string FileSizeProgress => $"{CompletedFileSize} / {TotalFileSize}";

    #endregion

    #region DirectoryCount

    private long _totalDirectoryCount;

    public long TotalDirectoryCount
    {
        get => _totalDirectoryCount;
        private set
        {
            _totalDirectoryCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DirectoryCountProgress));
        }
    }

    private long _completedDirectoryCount;

    public long CompletedDirectoryCount
    {
        get => _completedDirectoryCount;
        private set
        {
            _completedDirectoryCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DirectoryCountProgress));
        }
    }

    public string DirectoryCountProgress => $"{CompletedDirectoryCount} / {TotalDirectoryCount}";

    #endregion

    #region FileCount

    private long _totalFileCount;

    public long TotalFileCount
    {
        get => _totalFileCount;
        private set
        {
            _totalFileCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FileCountProgress));
        }
    }

    private long _completedFileCount;

    public long CompletedFileCount
    {
        get => _completedFileCount;
        private set
        {
            _completedFileCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FileCountProgress));
        }
    }

    public string FileCountProgress => $"{CompletedFileCount} / {TotalFileCount}";

    #endregion

    #region Percentage progress

    private long _completedPercentage;

    public long CompletedPercentage
    {
        get => _completedPercentage;
        private set
        {
            _completedPercentage = value;
            OnPropertyChanged();
        }
    }

    public const long TotalPercentage = 100;

    #endregion

    #region Current item

    private string _currentItem = "";

    public string CurrentItem
    {
        get => _currentItem;
        private set
        {
            _currentItem = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region CommandState

    private BackupCommandState _state;

    public BackupCommandState State
    {
        get => _state;
        private set
        {
            _state = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region CurrentOperation

    private CurrentOperationType? _currentOperationType;

    public CurrentOperationType? CurrentOperationType
    {
        get => _currentOperationType;
        private set
        {
            _currentOperationType = value;
            OnPropertyChanged();
        }
    }

    #endregion
}
