namespace BackupUtil.Core.Util;

internal static class Config
{
    // Default file path to look for a job file
    public static readonly string DefaultJobFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "EasySave", "Jobs", "BackupJobs.json");

    // Default max job count for the job manager
    public static uint DefaultMaxJobCount => uint.MaxValue;

    // Logging directory path
    public static string LoggingDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "EasySave", "Logs");
}
