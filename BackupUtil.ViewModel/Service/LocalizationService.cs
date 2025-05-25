using System.ComponentModel;
using System.Globalization;
using BackupUtil.I18n;

namespace BackupUtil.ViewModel.Service;

public class LocalizationService : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationService> s_instance = new(() => new LocalizationService());

    private LocalizationService() { }

    public static LocalizationService Instance => s_instance.Value;

    public string this[string key] => I18N.GetLocalizedMessage(key);

    public event PropertyChangedEventHandler? PropertyChanged;

    public void SetCulture(CultureInfo culture)
    {
        I18N.SetCulture(culture);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }

    public CultureInfo GetCurrentCulture()
    {
        return I18N.GetCurrentCulture();
    }

    public List<CultureInfo> GetSupportedCultures()
    {
        return I18N.GetSupportedCultures();
    }
}
