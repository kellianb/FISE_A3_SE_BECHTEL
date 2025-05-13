using System.Text.Json;

namespace BackupUtil.Core.Job.Exporter;

public static class JsonJobSerializer
{
    public static string Serialize(List<Job> jobs)
    {
        return JsonSerializer.Serialize(jobs);
    }
}
