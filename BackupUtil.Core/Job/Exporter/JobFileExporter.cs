using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job.Exporter;

public static class JobFileExporter
{
    public static void ExportJobsToFile(List<Job> jobs, string? filePath = null)
    {
        filePath = filePath != null
            ? Path.GetFullPath(filePath)
            : Path.GetFullPath(Config.DefaultJobFilePath);


        string serializedJobs = Path.GetExtension(filePath) switch
        {
            ".json" => JsonJobSerializer.Serialize(jobs),
            _ => throw new ArgumentException("errorFileFormatNotSupported")
        };

        File.WriteAllText(filePath, serializedJobs);
    }
}
