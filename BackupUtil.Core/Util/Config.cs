namespace BackupUtil.Core.Util;

public static class Config
{
    /// <summary>
    ///     Name of the application
    /// </summary>
    public const string AppName = "EasySave";

    /// <summary>
    ///     Default max job count for the job manager
    /// </summary>
    public static uint DefaultMaxJobCount => uint.MaxValue;

    #region File paths

    /// <summary>
    ///     Storage directory of the application
    /// </summary>
    public static readonly string AppDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);

    /// <summary>
    ///     Default file path to look for a job file
    /// </summary>
    public static readonly string DefaultJobFilePath = Path.Combine(AppDir, "Jobs", "BackupJobs.json");

    /// <summary>
    ///     Settings file path
    /// </summary>
    public static string SettingsFilePath { get; } = Path.Combine(AppDir, "Settings", "settings.json");

    /// <summary>
    ///     Logging directory
    /// </summary>
    public static string LoggingDirectory { get; } = Path.Combine(AppDir, "Logs");

    #endregion
}
