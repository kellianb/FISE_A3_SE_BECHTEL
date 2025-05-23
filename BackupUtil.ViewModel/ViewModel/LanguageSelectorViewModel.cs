using System.Collections.ObjectModel;
using System.Globalization;
using BackupUtil.I18n;

namespace BackupUtil.ViewModel.ViewModel;

public class LanguageSelectorViewModel : ViewModelBase
{
    private string _selectedLanguage = I18N.GetCurrentCulture().Name switch
    {
        "fr-FR" => I18N.GetLocalizedMessage("french"),
        _ => I18N.GetLocalizedMessage("english")
    };

    public ObservableCollection<string> AvailableLanguages { get; set; } = new()
    {
        I18N.GetLocalizedMessage("english"), I18N.GetLocalizedMessage("french")
    };

    public Dictionary<string, string> LocalizedMessages => new()
    {
        { "langLabel", I18N.GetLocalizedMessage("langLabel") }
    };

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }
    }

    public void ChangeLanguage(string language)
    {
        Console.WriteLine("TransmittedLanguage " + language);
        Console.WriteLine("frenchI18n " + I18N.GetLocalizedMessage("french"));
        Console.WriteLine("englishI18n " + I18N.GetLocalizedMessage("english"));
        string culture = language == I18N.GetLocalizedMessage("french") ? "fr-FR" : "en-US";
        Console.WriteLine("Culture " + culture);
        I18N.SetCulture(new CultureInfo(culture));
        //TODO: clean then display new language labels
        Console.WriteLine("SelectedLanguage before: " + SelectedLanguage);
        Console.WriteLine("AvailableLanguages before: " + AvailableLanguages);
        SelectedLanguage =
            culture == "fr-FR" ? I18N.GetLocalizedMessage("french") : I18N.GetLocalizedMessage("english");
        AvailableLanguages =
            new ObservableCollection<string>
            {
                I18N.GetLocalizedMessage("english"), I18N.GetLocalizedMessage("french")
            };
        Console.WriteLine("SelectedLanguage after: " + SelectedLanguage);
        Console.WriteLine("AvailableLanguages after: " + AvailableLanguages);
        AvailableLanguages.Clear();
        AvailableLanguages.Add(I18N.GetLocalizedMessage("english"));
        AvailableLanguages.Add(I18N.GetLocalizedMessage("french"));
        OnPropertyChanged(nameof(AvailableLanguages));
        OnPropertyChanged(nameof(LocalizedMessages));
        OnPropertyChanged(nameof(JobListingViewModel.LocalizedMessages));
        Console.WriteLine("AvailableLanguages after trigger: " + AvailableLanguages);
        Console.WriteLine("SelectedLanguage after trigger: " + SelectedLanguage);
    }
}
