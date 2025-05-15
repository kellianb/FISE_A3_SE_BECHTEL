using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job.Exporter;

internal static class JobFileExporter
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

        string? directoryName = Path.GetDirectoryName(filePath);

        // Create the directory if it doesn't exist
        if (!string.IsNullOrEmpty(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(filePath, serializedJobs);
    }
}
