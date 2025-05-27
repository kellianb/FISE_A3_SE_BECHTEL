using System.Globalization;
using System.Resources;

namespace BackupUtil.I18n;

public static class I18N
{
    private static readonly List<CultureInfo> s_supportedCultures =
    [
        new("fr-FR"), new("en-US")
    ];

    private static readonly ResourceManager s_resourceManager =
        new("BackupUtil.I18n.Resources.Strings", typeof(I18N).Assembly);

    public static string GetLocalizedMessage(string key)
    {
        try
        {
            return s_resourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
        }
        catch
        {
            return key;
        }
    }

    public static void SetCulture(CultureInfo culture)
    {
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    public static CultureInfo GetCurrentCulture()
    {
        return Thread.CurrentThread.CurrentUICulture;
    }

    public static List<CultureInfo> GetSupportedCultures()
    {
        return s_supportedCultures;
    }
}
