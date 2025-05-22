using System.Collections;
using System.Globalization;
using System.Resources;

namespace BackupUtil.I18n;

public static class I18N
{
    private static readonly List<CultureInfo> s_supportedCultures =
    [
        new("fr-FR"), new("en-GB")
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

    public static Dictionary<string, string> GetLocalizedMessages()
    {
        Dictionary<string, string> messages = new();
        ResourceSet? entries = s_resourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);

        if (entries == null)
        {
            return messages;
        }

        foreach (object a in entries)
        {
            string? key = a.ToString();
            string? value = entries.GetString(key ?? string.Empty);
            if (key == null || value == null)
            {
                continue;
            }

            messages.Add(a.ToString() ?? string.Empty, entries.GetString(a.ToString() ?? string.Empty) ?? string.Empty);
        }

        return messages;
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
