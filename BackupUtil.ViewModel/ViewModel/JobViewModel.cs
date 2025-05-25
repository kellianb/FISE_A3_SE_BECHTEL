using BackupUtil.Core.Job;

namespace BackupUtil.ViewModel.ViewModel;

public class JobViewModel(Job job) : ViewModelBase
{
    public string Name
    {
        get => job.Name;
        set
        {
            job.Name = value;
            OnPropertyChanged();
        }
    }

    public string SourcePath
    {
        get => job.SourcePath;
        set
        {
            job.SourcePath = value;
            OnPropertyChanged();
        }
    }

    public string TargetPath
    {
        get => job.TargetPath;
        set
        {
            job.TargetPath = value;
            OnPropertyChanged();
        }
    }

    public bool Recursive
    {
        get => job.Recursive;
        set
        {
            job.Recursive = value;
            OnPropertyChanged();
        }
    }

    public bool Differential
    {
        get => job.Differential;
        set
        {
            job.Differential = value;
            OnPropertyChanged();
        }
    }

    public EncryptionTypeOptions EncryptionType
    {
        get => EncryptionTypeOptionsUtils.From(job.EncryptionType);
        set
        {
            job.EncryptionType = EncryptionTypeOptionsUtils.To(value);
            OnPropertyChanged();
        }
    }

    public string EncryptionKey
    {
        get => job.EncryptionKey ?? "";
        set
        {
            job.EncryptionKey = value;
            OnPropertyChanged();
        }
    }

    public string FileMask
    {
        get => job.FileMask ?? "";
        set
        {
            job.FileMask = value;
            OnPropertyChanged();
        }
    }
}
