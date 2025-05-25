using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using BackupUtil.Core.Job;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Crypto;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class JobCreationViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private readonly JobManager _jobManager;

    public JobCreationViewModel(JobManager jobManager, NavigationService navigationService)
    {
        _jobManager = jobManager;

        SubmitCommand = new CreateJobCommand(this, jobManager, navigationService);
        CancelCommand = new NavigateCommand(navigationService);
    }

    public bool CanCreateJob => HasName
                                // Source path
                                && SourcePathExists
                                && SourcePathOutsideOfTargetPath
                                // Target path
                                && TargetPathDifferentFromSourcePath
                                && TargetPathOutsideOfSourcePath
                                && TargetPathSameTypeAsSourcePath
                                // Encryption
                                && EncryptionKeyOk
                                // File mask
                                && ValidFileMask;

    public ICommand SubmitCommand { get; }
    public ICommand CancelCommand { get; }


    #region Error handling

    private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary = new();

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == null ||
            !_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? errors))
        {
            return new List<string>();
        }

        return errors;
    }

    public bool HasErrors => _propertyNameToErrorsDictionary.Count != 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    private void AddError(string errorMessage, string propertyName)
    {
        if (!_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            _propertyNameToErrorsDictionary.Add(propertyName, value);
        }

        value.Add(errorMessage);

        OnErrorsChanged(propertyName);
    }

    private void ClearErrors(string propertyName)
    {
        _propertyNameToErrorsDictionary.Remove(propertyName);

        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion

    #region Name

    private bool HasName => !string.IsNullOrEmpty(Name);

    private string _name = "";

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));

            // Determine errors
            ClearErrors(nameof(Name));

            if (!HasName)
            {
                AddError("errorNameEmpty", nameof(Name));
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region SourcePath

    private string _sourcePath = new DirectoryInfo(".").FullName;

    private bool SourcePathExists => Directory.Exists(SourcePath) || File.Exists(SourcePath);

    private bool SourcePathOutsideOfTargetPath =>
        !SourcePath.StartsWith(TargetPath, StringComparison.OrdinalIgnoreCase);

    public string SourcePath
    {
        get => _sourcePath;
        set
        {
            _sourcePath = value;
            OnPropertyChanged(nameof(SourcePath));

            // Determine errors
            ClearErrors(nameof(SourcePath));

            if (!SourcePathExists)
            {
                AddError("errorSourcePath", nameof(SourcePath));
            }

            if (!SourcePathOutsideOfTargetPath)
            {
                AddError("errorSourceInTarget", nameof(SourcePath));
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region TargetPath

    private string _targetPath = new DirectoryInfo(".").FullName;

    private bool TargetPathDifferentFromSourcePath =>
        !string.Equals(SourcePath, TargetPath, StringComparison.OrdinalIgnoreCase);

    private bool TargetPathOutsideOfSourcePath =>
        !TargetPath.StartsWith(SourcePath, StringComparison.OrdinalIgnoreCase);

    // Source and target both have to be either files or directories
    private bool TargetPathSameTypeAsSourcePath => File.Exists(SourcePath)
        ? !Directory.Exists(TargetPath)
        : !File.Exists(TargetPath);

    public string TargetPath
    {
        get => _targetPath;
        set
        {
            _targetPath = value;
            OnPropertyChanged(nameof(TargetPath));

            // Determine errors
            ClearErrors(nameof(TargetPath));

            if (!TargetPathDifferentFromSourcePath)
            {
                AddError("errorSameSourceTarget", nameof(TargetPath));
            }

            if (!TargetPathOutsideOfSourcePath)
            {
                AddError("errorTargetInSource", nameof(TargetPath));
            }

            if (!TargetPathSameTypeAsSourcePath)
            {
                AddError(
                    File.Exists(SourcePath)
                        ? "errorSourceFileTargetDir"
                        : "errorSourceDirTargetFile", nameof(TargetPath));
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region Recursive

    private bool _recursive;

    public bool Recursive
    {
        get => _recursive;
        set
        {
            _recursive = value;
            OnPropertyChanged(nameof(Recursive));
        }
    }

    #endregion

    #region Differential

    private bool _differential;

    public bool Differential
    {
        get => _differential;
        set
        {
            _differential = value;
            OnPropertyChanged(nameof(Differential));
        }
    }

    #endregion

    #region EncryptionType

    private EncryptionType? _encryptionType;

    public EncryptionTypeOptions EncryptionType
    {
        get => EncryptionTypeOptionsUtils.From(_encryptionType);
        set
        {
            _encryptionType = EncryptionTypeOptionsUtils.To(value);
            OnPropertyChanged(nameof(EncryptionType));

            // Update encryption key error state
            ClearErrors(nameof(EncryptionKey));

            if (!EncryptionKeyOk)
            {
                // Determine errors
                AddError("errorEncryptionKeyEmpty", nameof(EncryptionKey));
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region EncryptionKey

    private string _encryptionKey = "";

    // Encryption key cannot be empty if an EncryptionType is set
    private bool EncryptionKeyOk =>
        EncryptionType == EncryptionTypeOptions.None || !string.IsNullOrEmpty(EncryptionKey);

    public string EncryptionKey
    {
        get => _encryptionKey;
        set
        {
            _encryptionKey = value;
            OnPropertyChanged(nameof(EncryptionKey));

            ClearErrors(nameof(EncryptionKey));

            if (!EncryptionKeyOk)
            {
                // Determine errors
                AddError("errorEncryptionKeyEmpty", nameof(EncryptionKey));
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region FileMask

    private string _fileMask = FileMaskBuilder.New().BuildSerialized();

    private bool ValidFileMask => string.IsNullOrWhiteSpace(FileMask) || FileMaskBuilder.ValidateSerialized(FileMask);

    public string FileMask
    {
        get => _fileMask;
        set
        {
            _fileMask = value;
            OnPropertyChanged(nameof(FileMask));

            ClearErrors(nameof(FileMask));

            if (!ValidFileMask)
            {
                // Determine errors
                AddError("errorInvalidFileMask", nameof(FileMask));
            }

            OnPropertyChanged(nameof(FileMask));
        }
    }

    #endregion
}
