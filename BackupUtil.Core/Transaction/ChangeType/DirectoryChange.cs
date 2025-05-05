namespace BackupUtil.Core.Operation.ChangeType;

public enum DirectoryChangeType
{
    Create,
    Delete
}

// Represents a directory that needs to be created or deleted
public class DirectoryChange(
    string targetPath,
    DirectoryChangeType changeType) : FileSystemChange(targetPath)
{
    public DirectoryChangeType ChangeType { get; } = changeType;
}
