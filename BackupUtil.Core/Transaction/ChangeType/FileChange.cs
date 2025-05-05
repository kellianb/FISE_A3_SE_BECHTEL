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
    : FileSystemChange(targetPath)
{
    public FileChangeType ChangeType { get; } = changeType;
    public long FileSize { get; } = fileSize;
    public string? SourcePath { get; } = sourcePath;
}
