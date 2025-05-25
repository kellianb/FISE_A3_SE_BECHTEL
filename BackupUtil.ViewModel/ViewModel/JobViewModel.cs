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
            OnPropertyChanged(nameof(Name));
        }
    }

    public string SourcePath
    {
        get => job.SourcePath;
        set
        {
            job.SourcePath = value;
            OnPropertyChanged(nameof(SourcePath));
        }
    }

    public string TargetPath
    {
        get => job.TargetPath;
        set
        {
            job.TargetPath = value;
            OnPropertyChanged(nameof(TargetPath));
        }
    }

    public bool Recursive
    {
        get => job.Recursive;
        set
        {
            job.Recursive = value;
            OnPropertyChanged(nameof(Recursive));
        }
    }

    public bool Differential
    {
        get => job.Differential;
        set
        {
            job.Differential = value;
            OnPropertyChanged(nameof(Differential));
        }
    }

    public EncryptionTypeOptions EncryptionType
    {
        get => EncryptionTypeOptionsUtils.From(job.EncryptionType);
        set
        {
            job.EncryptionType = EncryptionTypeOptionsUtils.To(value);
            OnPropertyChanged(nameof(EncryptionType));
        }
    }

    public string EncryptionKey
    {
        get => job.EncryptionKey ?? "";
        set
        {
            job.EncryptionKey = value;
            OnPropertyChanged(nameof(EncryptionKey));
        }
    }

    public string FileMask
    {
        get => job.FileMask ?? "";
        set
        {
            job.FileMask = value;
            OnPropertyChanged(nameof(FileMask));
        }
    }
}
