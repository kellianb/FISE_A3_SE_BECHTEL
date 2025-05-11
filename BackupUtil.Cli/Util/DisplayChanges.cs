using System.Text;
using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Util;

public class DisplayChanges
{
    public static string DisplayFileChanges(Dictionary<FileChangeType, string[]> changes)
    {
        if (changes.Count == 0)
        {
            return "";
        }

        StringBuilder changeDisplay = new();

        changeDisplay.AppendLine(I18N.GetLocalizedMessage("changesToFiles"));

        foreach (FileChangeType changeType in changes.Keys.OrderBy(k => (int)k))
        {
            changeDisplay.AppendLine(Formatting.BoldOn + I18nEnumExtensions.GetLocalizedMessage(changeType) +
                                     Formatting.Reset);

            foreach (string filePath in changes[changeType])
            {
                changeDisplay.AppendLine(Formatting.Indent + filePath);
            }

            changeDisplay.AppendLine();
        }

        return changeDisplay.ToString();
    }

    public static string DisplayDirectoryChanges(Dictionary<DirectoryChangeType, string[]> changes)
    {
        if (changes.Count == 0)
        {
            return "";
        }

        StringBuilder changeDisplay = new();

        changeDisplay.AppendLine(I18N.GetLocalizedMessage("changesToDirectories"));

        foreach (DirectoryChangeType changeType in changes.Keys.OrderBy(k => (int)k))
        {
            changeDisplay.AppendLine(Formatting.BoldOn + I18nEnumExtensions.GetLocalizedMessage(changeType) +
                                     Formatting.Reset);

            foreach (string filePath in changes[changeType])
            {
                changeDisplay.AppendLine(Formatting.Indent + filePath);
            }

            changeDisplay.AppendLine();
        }

        return changeDisplay.ToString();
    }
}
