namespace BackupUtil.Core.Job;

public class Job
{
    public Job(string sourcePath,
        string targetPath,
        bool recursive = false,
        bool differential = false,
        string? name = null,
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
        EncryptionKey = encryptionKey;
    }

    internal Job()
    {
        Name = "New backup job";
        SourcePath = "";
        TargetPath = "";
        Recursive = false;
        Differential = false;
        EncryptionKey = null;
        FileMask = null;
    }


    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }
    public bool Recursive { get; set; }
    public bool Differential { get; set; }
    public string? EncryptionKey { get; set; }
    public string? FileMask { get; set; }

}
