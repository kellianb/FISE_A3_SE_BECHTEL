using System.Text.Json.Serialization;
using BackupUtil.Crypto;

namespace BackupUtil.Core.Job;

public class Job
{
    public Job(string sourcePath,
        string targetPath,
        bool recursive = false,
        bool differential = false,
        string? name = null,
        EncryptionType? encryptionType = null,
        string? encryptionKey = null,
        string? fileMask = null
    )
    {
        Name = name ?? "New backup job";
        SourcePath = sourcePath;
        TargetPath = targetPath;
        FileMask = fileMask;
        Recursive = recursive;
        Differential = differential;
        EncryptionType = encryptionType;
        EncryptionKey = encryptionKey;
    }

    internal Job()
    {
        Name = "New backup job";
        SourcePath = "";
        TargetPath = "";
        Recursive = false;
        Differential = false;
        EncryptionType = null;
        EncryptionKey = null;
        FileMask = null;
    }


    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public bool Recursive { get; set; }
    public bool Differential { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<EncryptionType>))]
    public EncryptionType? EncryptionType { get; set; }
    public string? EncryptionKey { get; set; }
    public string? FileMask { get; set; }
}
