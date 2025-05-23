using System.Collections;
using System.ComponentModel;
using BackupUtil.Crypto;

namespace BackupUtil.ViewModel.ViewModel;

public class JobCreationViewModel : ViewModelBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary = new();

    public bool CanCreateJob => HasName
                                && TargetPathDifferentFromSourcePath
                                && TargetPathOutsideOfSourcePath
                                && TargetPathSameTypeAsSourcePath
                                && SourcePathOutsideOfTargetPath
                                && EncryptionKeyOk;

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

    private FileSystemInfo _sourcePath = new DirectoryInfo(".");

    private bool SourcePathOutsideOfTargetPath =>
        !_sourcePath.FullName.StartsWith(_targetPath.FullName, StringComparison.OrdinalIgnoreCase);

    public string SourcePath
    {
        get => _sourcePath.FullName;
        set
        {
            if (File.Exists(value))
            {
                _sourcePath = new FileInfo(value);
            }
            else if (Directory.Exists(value))
            {
                _sourcePath = new DirectoryInfo(value);
            }
            else
            {
                _sourcePath = new DirectoryInfo(".");
            }

            if (!SourcePathOutsideOfTargetPath)
            {

            }

            OnPropertyChanged(nameof(SourcePath));
        }
    }

    #endregion

    #region TargetPath

    private FileSystemInfo _targetPath = new DirectoryInfo(".");

    private bool TargetPathDifferentFromSourcePath =>
        !string.Equals(SourcePath, TargetPath, StringComparison.OrdinalIgnoreCase);

    private bool TargetPathOutsideOfSourcePath =>
        !TargetPath.StartsWith(SourcePath, StringComparison.OrdinalIgnoreCase);

    // Source and target both have to be either files or directories
    private bool TargetPathSameTypeAsSourcePath => _sourcePath.Attributes.HasFlag(FileAttributes.Directory) ==
                                                   _targetPath.Attributes.HasFlag(FileAttributes.Directory);

    public string TargetPath
    {
        get => _targetPath.FullName;
        set
        {
            if (File.Exists(value))
            {
                _targetPath = new FileInfo(value);
            }
            else if (Directory.Exists(value))
            {
                _targetPath = new DirectoryInfo(value);
            }
            else
            {
                _targetPath = new DirectoryInfo(".");
            }

            OnPropertyChanged(nameof(TargetPath));
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

    public EncryptionType? EncryptionType
    {
        get => _encryptionType;
        set
        {
            _encryptionType = value;
            OnPropertyChanged(nameof(EncryptionType));
        }
    }

    #endregion

    #region EncryptionKey

    private string _encryptionKey = "";

    // Encryption key cannot be empty if EncryptionType is set
    private bool EncryptionKeyOk => EncryptionType == null || !string.IsNullOrEmpty(EncryptionKey);

    public string EncryptionKey
    {
        get => _encryptionKey;
        set
        {
            _encryptionKey = value;
            OnPropertyChanged(nameof(EncryptionKey));
        }
    }

    #endregion

    #region FileMask

    private string _fileMask = "";

    public string FileMask
    {
        get => _fileMask;
        set
        {
            _fileMask = value;
            OnPropertyChanged(nameof(FileMask));
        }
    }

    #endregion
}
