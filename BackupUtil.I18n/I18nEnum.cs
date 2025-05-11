using System.Reflection;

namespace BackupUtil.I18n;

/// <summary>
///     Add corresponding I18n keys enum values
/// </summary>
/// <param name="key">I18n key</param>
[AttributeUsage(AttributeTargets.Field)]
public class I18NKeyAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}

/// <summary>
///     Get the corresponding I18n key from an enum value
/// </summary>
public static class I18nEnumExtensions
{
    /// <summary>
    ///     Get the corresponding I18n key from an enum value
    /// </summary>
    public static string GetI18nKey(Enum value)
    {
        Type type = value.GetType();

        FieldInfo? fieldInfo = type.GetField(value.ToString());

        I18NKeyAttribute? attribute = fieldInfo?.GetCustomAttributes(typeof(I18NKeyAttribute), false)
            .FirstOrDefault() as I18NKeyAttribute;

        return attribute?.Key ?? value.ToString();
    }

    public static string GetLocalizedMessage(Enum value)
    {
        return I18N.GetLocalizedMessage(GetI18nKey(value));
    }
}
