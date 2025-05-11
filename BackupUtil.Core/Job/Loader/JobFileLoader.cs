namespace BackupUtil.Core.Job.Loader;

public static class JobFileLoader
{
    public static List<Job> LoadJobsFromFile(string filePath)
    {
        filePath = Path.GetFullPath(filePath);

        using StreamReader reader = new(filePath);

        return Path.GetExtension(filePath) switch
        {
            ".json" => JsonDeserializer.LoadJobs(reader),
            _ => throw new ArgumentException("errorFileFormatNotSupported")
        };
    }
}
