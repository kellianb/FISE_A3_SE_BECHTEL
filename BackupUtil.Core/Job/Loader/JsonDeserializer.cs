using System.Text.Json;

namespace BackupUtil.Core.Job.Loader;

public static class JsonDeserializer
{
    public static List<Job> Deserialize(TextReader reader)
    {
        return JsonSerializer.Deserialize<List<Job>>(reader.ReadToEnd()) ?? [];
    }
}
