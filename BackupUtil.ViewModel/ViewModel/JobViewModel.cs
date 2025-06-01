using BackupUtil.Core.Job;

namespace BackupUtil.ViewModel.ViewModel;

public class JobViewModel(Job job) : ViewModelBase
{
    #region Name

    public string Name
    {
        get => job.Name;
        private set
        {
            job.Name = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region SourcePath

    public string SourcePath
    {
        get => job.SourcePath;
        private set
        {
            job.SourcePath = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region TargetPath

    public string TargetPath
    {
        get => job.TargetPath;
        private set
        {
            job.TargetPath = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Recursive

    public bool Recursive
    {
        get => job.Recursive;
        private set
        {
            job.Recursive = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Differential

    public bool Differential
    {
        get => job.Differential;
        private set
        {
            job.Differential = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region EncryptionType

    public EncryptionTypeOptions EncryptionType
    {
        get => EncryptionTypeOptionsUtils.From(job.EncryptionType);
        private set
        {
            job.EncryptionType = EncryptionTypeOptionsUtils.To(value);
            OnPropertyChanged();
        }
    }

    #endregion

    #region EncryptionKey

    public string EncryptionKey
    {
        get => job.EncryptionKey ?? "";
        private set
        {
            job.EncryptionKey = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region FileMask

    public string FileMask
    {
        get => job.FileMask ?? "";
        private set
        {
            job.FileMask = value;
            OnPropertyChanged();
        }
    }

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
}
