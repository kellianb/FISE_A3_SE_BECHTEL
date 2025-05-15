using System.Text.Json;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job.Loader;

internal static class JsonJobDeserializer
{
    public static List<Job> Deserialize(TextReader reader)
    {
        return JsonSerializer.Deserialize(reader.ReadToEnd(), JsonBackupUtilSerializerContext.Default.ListJob) ?? [];
    }
}
