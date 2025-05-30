using System.ComponentModel;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.Settings;

public class ApplySettingsCommand<TViewModel> : CommandBase where TViewModel : ViewModelBase
{
    private readonly LocalizationService _localizationService;
    private readonly NavigationService<TViewModel> _navigationService;
    private readonly ProgramFilterStore _programFilterStore;
    private readonly SettingsViewModel _settingsViewModel;

    public ApplySettingsCommand(
        SettingsViewModel settingsViewModel,
        LocalizationService localizationService,
        ProgramFilterStore programFilterStore,
        NavigationService<TViewModel> navigationService)
    {
        _navigationService = navigationService;
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
        // Set language
        _localizationService.SetCulture(_settingsViewModel.SelectedLanguage);

        // Set banned programs
        _programFilterStore.BannedPrograms =
        [
            .._settingsViewModel.BannedPrograms.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        ];

        _navigationService.Navigate();
    }
}
