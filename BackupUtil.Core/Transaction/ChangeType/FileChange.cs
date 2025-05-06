namespace BackupUtil.Core.Transaction.ChangeType;

public enum FileChangeType
{
    Create,
    Modify,
    Delete
}

// Represents a file that needs to be created, modified, or deleted
public class FileChange(
    string targetPath,
    FileChangeType changeType,
    string? sourcePath = null,
    long fileSize = 0)
    : FileSystemChange(targetPath),
        IEquatable<FileChange>
{
    public FileChangeType ChangeType { get; } = changeType;
    public long FileSize { get; } = fileSize;
    public string? SourcePath { get; } = sourcePath;

    public bool Equals(FileChange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ChangeType == other.ChangeType && FileSize == other.FileSize && SourcePath == other.SourcePath;
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

        return Equals((FileChange)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)ChangeType, FileSize, SourcePath);
    }
}
