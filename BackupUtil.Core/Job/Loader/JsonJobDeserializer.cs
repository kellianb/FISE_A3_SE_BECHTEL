using System.Text.Json;

namespace BackupUtil.Core.Job.Loader;

internal static class JsonJobDeserializer
{
    public static List<Job> Deserialize(TextReader reader)
    {
        return JsonSerializer.Deserialize<List<Job>>(reader.ReadToEnd()) ?? [];
    }
}
