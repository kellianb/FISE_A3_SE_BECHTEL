namespace BackupUtil.Core.Util;

public class Config
{
    // Default file path to look for a job file
    public static string DefaultJobFilePath { get; } = "BackupJobs.json";

    // Default max job count for the job manager
    public static uint DefaultMaxJobCount { get; } = 5;
}
