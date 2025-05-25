using System.Collections.ObjectModel;
using System.Globalization;
using BackupUtil.ViewModel.Service;

namespace BackupUtil.ViewModel.ViewModel;

public class LanguageSelectionViewModel : ViewModelBase
{
    private readonly LocalizationService _localizationService = LocalizationService.Instance;

    public LanguageSelectionViewModel()
    {
        AvailableLanguages = new ObservableCollection<CultureInfo>(_localizationService.GetSupportedCultures());
    }

    public ObservableCollection<CultureInfo> AvailableLanguages { get; set; }

    public CultureInfo SelectedLanguage
    {
        get => _localizationService.GetCurrentCulture();
        set
        {
            _localizationService.SetCulture(value);
            OnPropertyChanged();
        }
    }
}
