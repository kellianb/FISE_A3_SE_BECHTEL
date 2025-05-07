using System.Globalization;
using System.Reflection;
using System.Resources;

namespace BackupUtil.Core.Util;

public static class I18N
{
    private static readonly ResourceManager ResourceManager =
        new ResourceManager("BackupUtil.Core.Resources.Strings", typeof(I18N).Assembly);

    public static string GetLocalizedMessage(string key)
    {
        return ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
