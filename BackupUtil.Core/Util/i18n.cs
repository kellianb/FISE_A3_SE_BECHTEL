using System.Globalization;
using System.Reflection;
using System.Resources;

namespace BackupUtil.Core.Util;

public static class I18N
{
    private static readonly ResourceManager s_resourceManager =
        new ResourceManager("BackupUtil.Core.Resources.Strings", typeof(I18N).Assembly);

    public static string GetLocalizedMessage(string key)
    {
        return s_resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
