using System.ComponentModel;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.Settings;

public class ApplySettingsCommand<TViewModel> : CommandBase where TViewModel : ViewModelBase
{
    private readonly AppSettingsStore _appSettingsStore;
    private readonly LocalizationService _localizationService;
    private readonly NavigationService<TViewModel> _navigationService;
    private readonly ProgramFilterStore _programFilterStore;
    private readonly SettingsViewModel _settingsViewModel;

    public ApplySettingsCommand(
        SettingsViewModel settingsViewModel,
        LocalizationService localizationService,
        ProgramFilterStore programFilterStore,
        NavigationService<TViewModel> navigationService,
        AppSettingsStore appSettingsStore)
    {
        _navigationService = navigationService;
        _appSettingsStore = appSettingsStore;
        _localizationService = localizationService;
        _programFilterStore = programFilterStore;
        _settingsViewModel = settingsViewModel;
        _settingsViewModel.PropertyChanged += OnViewModelProperyChanged;
    }

    private void OnViewModelProperyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_settingsViewModel.CanSaveSettings))
        {
            OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return _settingsViewModel.CanSaveSettings && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        try
        {
            // Set language
            _localizationService.SetCulture(_settingsViewModel.SelectedLanguage);

            // Set banned programs
            _programFilterStore.BannedPrograms =
            [
                .._settingsViewModel.BannedPrograms.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            ];

            _appSettingsStore.BannedPrograms = _programFilterStore.BannedPrograms;
            _appSettingsStore.Culture = _settingsViewModel.SelectedLanguage.Name;
            _appSettingsStore.SaveToJsonFile(Config.SettingsFilePath);
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }

        _navigationService.Navigate();
    }
}
