using System.Text.Json.Serialization;
using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Util;

[JsonSerializable(typeof(Job.Job))]
[JsonSerializable(typeof(List<Job.Job>))]
[JsonSerializable(typeof(BackupTransaction))]
internal partial class JsonBackupUtilSerializerContext : JsonSerializerContext
{
}
