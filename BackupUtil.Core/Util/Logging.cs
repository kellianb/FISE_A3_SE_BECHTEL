using System.Text.Json;
using BackupUtil.Core.Transaction;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace BackupUtil.Core.Util;

public static class Logging
{
    public static void Init()
    {
        Log.Logger = GetLogger();

        Log.Debug("Logging to: {string}", Path.Join(Config.LoggingDirectory, $"{DateTime.Now:yyyy-MM-dd}.json"));
    }

    private static ILogger GetLogger()
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
            .CreateLogger();
    }
}
