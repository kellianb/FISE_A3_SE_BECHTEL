using System.Globalization;
using System.Text.Json;
using BackupUtil.Core.Util;

namespace BackupUtil.ViewModel.Store;

public class AppSettingsStore
{
    public List<string>? BannedPrograms { get; set; } = [];
    public string? Culture { get; set; } = "";

    public CultureInfo? GetCulture()
    {
        if (string.IsNullOrEmpty(Culture))
            return null;

        try
        {
            return CultureInfo.GetCultureInfo(Culture);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    public static AppSettingsStore LoadFromJsonFile(string path)
    {
        try
        {
            string jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppSettingsStore>(jsonString) ?? new AppSettingsStore();
        }
        catch (Exception)
        {
            return new AppSettingsStore();
        }
    }

    public bool SaveToJsonFile(string path)
    {
        try
        {
            JsonSerializerOptions options = new() { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(this, options);

            string? directoryName = Path.GetDirectoryName(path);

            // Create the directory if it doesn't exist
            if (!string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            File.WriteAllText(path, jsonString);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
