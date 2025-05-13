namespace BackupUtil.Core.Job.Loader;

public static class JobFileLoader
{
    public static List<Job> LoadJobsFromFile(string? filePath)
    {
        filePath = filePath != null
            ? Path.GetFullPath(filePath)
            : Path.GetFullPath("BackupJobs.json"); // Use a default path in CWD by default

        using StreamReader reader = new(filePath);

        return Path.GetExtension(filePath) switch
        {
            ".json" => JsonJobDeserializer.Deserialize(reader),
            _ => throw new ArgumentException("errorFileFormatNotSupported")
        };
    }
}
