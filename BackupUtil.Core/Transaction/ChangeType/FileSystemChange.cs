namespace BackupUtil.Core.Operation.ChangeType;

// Base class for both file and directory changes
public abstract class FileSystemChange(string targetPath)
{
    public string TargetPath { get; } = targetPath;
}
