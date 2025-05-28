using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.ViewModel;

public class TransactionViewModel : ViewModelBase
{
    public TransactionViewModel(BackupCommand command)
    {
        JobNames = string.Join(", ", command.JobNames);
        TotalCopiedFileSize = command.TotalFileSize;
    }

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
        set
        {
            _jobNames = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region TotalCopiedFileSize

    private long _totalCopiedFileSize;

    public long TotalCopiedFileSize
    {
        get => _totalCopiedFileSize;
        set
        {
            _totalCopiedFileSize = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region CommandState

    // TODO add backup command status

    #endregion
}
