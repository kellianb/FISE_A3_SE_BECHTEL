using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job.Loader;

internal static class JobFileLoader
{
    public static List<Job> LoadJobsFromFile(string? filePath)
    {
        filePath = filePath != null
            ? Path.GetFullPath(filePath)
            : Path.GetFullPath(Config.DefaultJobFilePath);

        using StreamReader reader = new(filePath);

        return Path.GetExtension(filePath) switch
        {
            ".json" => JsonJobDeserializer.Deserialize(reader),
            _ => throw new ArgumentException("errorFileFormatNotSupported")
        };
    }
}
