using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Crypto;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Command.CreateJob;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class JobCreationViewModel : ViewModelBase, INotifyDataErrorInfo
{
    public JobCreationViewModel(JobStore jobStore, NavigationService<HomeViewModel> navigationService)
    {
        SubmitCommand = new CreateJobCommand<HomeViewModel>(this, jobStore, navigationService);
        CancelCommand = new NavigateCommand<HomeViewModel>(navigationService);
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

    #region Commands

    // Submit the job creation and navigate back to the job listing view model
    public ICommand SubmitCommand { get; }

    // Navigate back to the job listing view model
    public ICommand CancelCommand { get; }

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
            OnPropertyChanged();

            // Determine errors
            ClearErrors();

            if (!HasName)
            {
                AddError("errorNameEmpty");
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region SourcePath

    private string _sourcePath = new DirectoryInfo(".").FullName;

    private bool SourcePathExists => Directory.Exists(SourcePath) || File.Exists(SourcePath);

    private bool SourcePathOutsideOfTargetPath =>
        !SourcePath.StartsWith(TargetPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    public string SourcePath
    {
        get => _sourcePath;
        set
        {
            _sourcePath = value;
            OnPropertyChanged();

            // Determine errors
            ClearErrors();

            if (!SourcePathExists)
            {
                AddError("errorSourcePath");
            }

            if (!SourcePathOutsideOfTargetPath)
            {
                AddError("errorSourceInTarget");
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
        !TargetPath.StartsWith(SourcePath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

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
            OnPropertyChanged();

            // Determine errors
            ClearErrors();

            if (!TargetPathDifferentFromSourcePath)
            {
                AddError("errorSameSourceTarget");
            }

            if (!TargetPathOutsideOfSourcePath)
            {
                AddError("errorTargetInSource");
            }

            if (!TargetPathSameTypeAsSourcePath)
            {
                AddError(
                    File.Exists(SourcePath)
                        ? "errorSourceFileTargetDir"
                        : "errorSourceDirTargetFile");
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
            OnPropertyChanged();
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
            OnPropertyChanged();
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
            OnPropertyChanged();

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
            OnPropertyChanged();

            ClearErrors();

            if (!EncryptionKeyOk)
            {
                // Determine errors
                AddError("errorEncryptionKeyEmpty");
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

    #region FileMask

    private string _fileMask = FileMaskBuilder.Default().BuildSerialized();

    private bool ValidFileMask => string.IsNullOrWhiteSpace(FileMask) || FileMaskBuilder.ValidateSerialized(FileMask);

    public string FileMask
    {
        get => _fileMask;
        set
        {
            _fileMask = value;
            OnPropertyChanged();

            ClearErrors();

            if (!ValidFileMask)
            {
                // Determine errors
                AddError("errorInvalidFileMask");
            }

            OnPropertyChanged(nameof(CanCreateJob));
        }
    }

    #endregion

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

    private void AddError(string errorMessage, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        if (!_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            _propertyNameToErrorsDictionary.Add(propertyName, value);
        }

        value.Add(errorMessage);

        OnErrorsChanged(propertyName);
    }

    private void ClearErrors([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        _propertyNameToErrorsDictionary.Remove(propertyName);

        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion
}
