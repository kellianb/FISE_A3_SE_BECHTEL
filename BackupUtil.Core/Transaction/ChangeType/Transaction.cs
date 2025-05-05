namespace BackupUtil.Core.Operation.ChangeType;

// Class to hold all the changes that need to be applied
public class Transaction
{
    private List<FileChange> FileChanges { get; } = [];
    private List<DirectoryChange> DirectoryChanges { get; } = [];

    public long GetTotalFileSize()
    {
        return FileChanges.Sum(x => x.ChangeType == FileChangeType.Delete ? 0 : x.FileSize);
    }

    #region File Changes

    private Transaction AddFileChange(FileChange change)
    {
        FileChanges.Add(change);
        return this;
    }

    public Transaction AddFileCreation(FileInfo sourceFile, string targetFilePath)
    {
        FileChange change = new(targetFilePath, FileChangeType.Create, sourceFile.FullName, sourceFile.Length);
        return AddFileChange(change);
    }

    public Transaction AddFileUpdate(FileInfo sourceFile, FileInfo targetFile)
    {
        FileChange change = new(targetFile.FullName, FileChangeType.Modify, sourceFile.FullName, sourceFile.Length);

        return AddFileChange(change);
    }

    public Transaction AddFileDeletion(FileInfo targetFile)
    {
        FileChange change = new(targetFile.FullName, FileChangeType.Delete);
        return AddFileChange(change);
    }

    #endregion

    #region Directory Changes

    private Transaction AddDirectoryChange(DirectoryChange change)
    {
        DirectoryChanges.Add(change);
        return this;
    }

    public Transaction AddDirectoryCreation(string targetDirectoryPath)
    {
        DirectoryChange change = new(targetDirectoryPath, DirectoryChangeType.Create);
        return AddDirectoryChange(change);
    }

    public Transaction AddDirectoryDeletion(DirectoryInfo targetDirectory)
    {
        DirectoryChange change = new(targetDirectory.FullName, DirectoryChangeType.Delete);
        return AddDirectoryChange(change);
    }

    #endregion
}
