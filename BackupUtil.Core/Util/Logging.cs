using System.Text.Json;
using BackupUtil.Core.Transaction;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace BackupUtil.Core.Util;

public static class Logging
{
    public static readonly Lazy<ILogger> DailyLog = new(GetDailyLogger);
    public static readonly Lazy<ILogger> StatusLog = new(GetStatusLogger) ;

    /// <summary>
    /// Get the logger for the daily log file
    /// </summary>
    /// <returns></returns>
    private static ILogger GetDailyLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Destructure.ByTransforming<BackupTransaction>(t =>
                JsonSerializer.Serialize(t, JsonBackupUtilSerializerContext.Default.BackupTransaction))
            .WriteTo.Console(LogEventLevel.Warning)
            .WriteTo.File(
                path: Path.Join(Config.LoggingDirectory, "log-.json"),
                rollingInterval: RollingInterval.Day,
                formatter: new JsonFormatter(),
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.File(
                path: Path.Join(Config.LoggingDirectory, "log-.xml"),
                rollingInterval: RollingInterval.Day,
                formatter: new XmlLogFormatter(),
                restrictedToMinimumLevel: LogEventLevel.Information
                )
            .CreateLogger();
    }


    /// <summary>
    /// Get the logger for the status log file
    /// </summary>
    /// <returns></returns>
    private static ILogger GetStatusLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Destructure.ByTransforming<BackupTransaction>(t =>
                JsonSerializer.Serialize(t, JsonBackupUtilSerializerContext.Default.BackupTransaction))
            .WriteTo.Console(LogEventLevel.Warning)
            .WriteTo.File(
                path: Path.Join(Config.LoggingDirectory, "status.json"),
                formatter: new JsonFormatter(),
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.File(
                path: Path.Join(Config.LoggingDirectory, "status.xml"),
                formatter: new XmlLogFormatter(),
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .CreateLogger();
    }
}
