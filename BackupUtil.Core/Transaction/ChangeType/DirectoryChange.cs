namespace BackupUtil.Core.Transaction.ChangeType;

public enum DirectoryChangeType
{
    Create,
    Delete
}

// Represents a directory that needs to be created or deleted
public class DirectoryChange(
    string targetPath,
    DirectoryChangeType changeType) : FileSystemChange(targetPath),
    IEquatable<DirectoryChange>
{
    public DirectoryChangeType ChangeType { get; } = changeType;

    public bool Equals(DirectoryChange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ChangeType == other.ChangeType;
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

        return Equals((DirectoryChange)obj);
    }

    public override int GetHashCode()
    {
        return (int)ChangeType;
    }
}
