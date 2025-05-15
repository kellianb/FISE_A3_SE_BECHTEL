using System.Text.Json;
using System.Text.Json.Serialization;
using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Util;

[JsonSerializable(typeof(Job.Job))]
[JsonSerializable(typeof(List<Job.Job>))]
[JsonSerializable(typeof(BackupTransaction))]
internal partial class JsonBackupUtilSerializerContext : JsonSerializerContext
{
}

public class UncPathJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // During deserialization, we'll just read the path as-is
        return reader.GetString() ?? string.Empty;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        // During serialization, convert the path to UNC format
        writer.WriteStringValue(UncPathConverter.GetUncPath(value));
    }
}
