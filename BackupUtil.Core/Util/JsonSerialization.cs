using System.Text.Json.Serialization;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Transaction.FileMask;

namespace BackupUtil.Core.Util;

[JsonSerializable(typeof(Job.Job))]
[JsonSerializable(typeof(FileMask))]
[JsonSerializable(typeof(List<Job.Job>))]
[JsonSerializable(typeof(BackupTransaction))]
internal partial class JsonBackupUtilSerializerContext : JsonSerializerContext
{
}
