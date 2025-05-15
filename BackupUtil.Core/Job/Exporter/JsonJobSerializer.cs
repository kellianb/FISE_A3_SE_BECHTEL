using System.Text.Json;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job.Exporter;

internal static class JsonJobSerializer
{
    public static string Serialize(List<Job> jobs)
    {
        return JsonSerializer.Serialize(jobs, JsonBackupUtilSerializerContext.Default.ListJob);
    }
}
