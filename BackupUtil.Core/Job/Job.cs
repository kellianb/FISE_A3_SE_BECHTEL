namespace BackupUtil.Core.Job;

public class Job : IEquatable<Job>
{
    public Job(string sourcePath,
        string targetPath,
        bool recursive = false,
        bool differential = false,
        string? name = null,
        string? encryptionKey = null
    )
    {
        Name = name ?? "New backup job";
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Recursive = recursive;
        Differential = differential;
        EncryptionKey = encryptionKey;
    }

    public Job()
    {
        Name = "New backup job";
        SourcePath = "";
        TargetPath = "";
        Recursive = false;
        Differential = false;
        EncryptionKey = null;
    }


    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }

    public bool Recursive { get; set; }
    public bool Differential { get; set; }
    public string? EncryptionKey { get; set; }

    #region Equality

    public bool Equals(Job? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Name == other.Name && SourcePath == other.SourcePath && TargetPath == other.TargetPath &&
               Recursive == other.Recursive && Differential == other.Differential &&
               EncryptionKey == other.EncryptionKey;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((Job)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, SourcePath, TargetPath, Recursive, Differential, EncryptionKey);
    }

    #endregion
}
