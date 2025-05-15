using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using BackupUtil.I18n;

namespace BackupUtil.ViewModel;

public class LanguageSelectorViewModel
{
    public ObservableCollection<string> AvailableLanguages { get; } = new ObservableCollection<string> { I18N.GetLocalizedMessage("english"), I18N.GetLocalizedMessage("french") };

    private string _selectedLanguage = I18N.GetCurrentCulture().Name switch
    {
        "fr-FR" => I18N.GetLocalizedMessage("french"),
        _ => I18N.GetLocalizedMessage("english")
    };

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                ChangeLanguage(value);
            }
        }
    }

    private void ChangeLanguage(string language)
    {
        string culture = language == "French" ? "fr-FR" : "en-US";
        I18N.SetCulture(new CultureInfo(culture));
        //TODO: clean then display new language labels
    }
}
