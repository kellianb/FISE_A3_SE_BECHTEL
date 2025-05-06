namespace BackupUtil.Core.Transaction.ChangeType;

// Base class for both file and directory changes
public abstract class FileSystemChange(string targetPath): IEquatable<FileSystemChange>
{
    public string TargetPath { get; } = targetPath;

    public bool Equals(FileSystemChange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return TargetPath == other.TargetPath;
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

        return Equals((FileSystemChange)obj);
    }

    public override int GetHashCode()
    {
        return TargetPath.GetHashCode();
    }
}
