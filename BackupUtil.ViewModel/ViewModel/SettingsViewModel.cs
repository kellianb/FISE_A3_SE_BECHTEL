using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class SettingsViewModel : ViewModelBase
{
    private readonly LocalizationService _localizationService = LocalizationService.Instance;

    private readonly ProgramFilterStore _programFilterStore;

    public SettingsViewModel(ProgramFilterStore programFilterStore,
        NavigationService<HomeViewModel> navigationService)
    {
        _programFilterStore = programFilterStore;

        // Fetch selected languages
        _selectedLanguage = _localizationService.GetCurrentCulture();

        // Fetch banned programs
        _bannedPrograms = string.Join(Environment.NewLine, _programFilterStore.BannedPrograms);

        ApplyCommand = new ApplySettingsCommand<HomeViewModel>(this,
            _localizationService,
            _programFilterStore,
            navigationService);

        CancelCommand = new NavigateCommand<HomeViewModel>(navigationService);
    }

    public bool CanSaveSettings => true;


    #region Commands

    // Apply the settings and navigate back to the job listing view model
    public ICommand ApplyCommand { get; }

    // Navigate back to the job listing view model
    public ICommand CancelCommand { get; }

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

    #region Language

    public List<CultureInfo> AvailableLanguages => _localizationService.GetSupportedCultures();

    private CultureInfo _selectedLanguage;

    public CultureInfo SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            _selectedLanguage = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region ProgramFilter

    private string _bannedPrograms;

    // Banned programs are represented as one entry per line in a textbox
    public string BannedPrograms
    {
        get => _bannedPrograms;
        set
        {
            _bannedPrograms = value;
            OnPropertyChanged();
        }
    }

    #endregion
}
