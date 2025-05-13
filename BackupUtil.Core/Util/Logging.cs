using System.Text.Json;
using BackupUtil.Core.Transaction;
using Serilog;
using Serilog.Formatting.Json;

namespace BackupUtil.Core.Util;

public static class Logging
{
    public static void Init()
    {
        Log.Logger = GetLogger();
    }

    private static ILogger GetLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Destructure.ByTransforming<BackupTransaction>(t => JsonSerializer.Serialize(t))
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Join(Config.LoggingDirectory, $"{DateTime.Now:yyyy-MM-dd}.json"),
                rollingInterval: RollingInterval.Day,
                formatter: new JsonFormatter()
            )
            .CreateLogger();
    }
}
