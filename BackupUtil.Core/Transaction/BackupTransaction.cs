using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Transaction;

// Class to hold all the changes that need to be applied
public class BackupTransaction
{
    public List<FileChange> FileChanges { get; } = [];
    public List<DirectoryChange> DirectoryChanges { get; } = [];


    #region Get info about file changes
    public long GetTotalFileSize()
    {
        return FileChanges.Sum(x => x.ChangeType == FileChangeType.Delete ? 0 : x.FileSize);
    }

    public long GetTotalCreatedFiles()
    {
        return FileChanges.Count(x => x.ChangeType == FileChangeType.Create);
    }

    public long GetTotalModifiedFiles()
    {
        return FileChanges.Count(x => x.ChangeType == FileChangeType.Modify);
    }

    public long GetTotalDeletedFiles()
    {
        return FileChanges.Count(x => x.ChangeType == FileChangeType.Delete);
    }
    #endregion

    #region Add file Changes

    private BackupTransaction AddFileChange(FileChange change)
    {
        FileChanges.Add(change);
        return this;
    }

    public BackupTransaction AddFileCreation(FileInfo sourceFile, string targetFilePath)
    {
        FileChange change = new(targetFilePath, FileChangeType.Create, sourceFile.FullName, sourceFile.Length);
        return AddFileChange(change);
    }

    public BackupTransaction AddFileUpdate(FileInfo sourceFile, FileInfo targetFile)
    {
        FileChange change = new(targetFile.FullName, FileChangeType.Modify, sourceFile.FullName, sourceFile.Length);

        return AddFileChange(change);
    }

    public BackupTransaction AddFileDeletion(FileInfo targetFile)
    {
        FileChange change = new(targetFile.FullName, FileChangeType.Delete);
        return AddFileChange(change);
    }

    #endregion

    #region Get info about directory changes
    public long GetTotalCreatedDirectories()
    {
        return DirectoryChanges.Count(x => x.ChangeType == DirectoryChangeType.Create);
    }

    public long GetTotalDeletedDirectories()
    {
        return DirectoryChanges.Count(x => x.ChangeType == DirectoryChangeType.Delete);
    }
    #endregion

    #region Add directory Changes

    private BackupTransaction AddDirectoryChange(DirectoryChange change)
    {
        DirectoryChanges.Add(change);
        return this;
    }

    public BackupTransaction AddDirectoryCreation(string path)
    {
        DirectoryChange change = new(path, DirectoryChangeType.Create);
        return AddDirectoryChange(change);
    }

    public BackupTransaction AddDirectoryDeletion(DirectoryInfo targetDirectory)
    {
        DirectoryChange change = new(targetDirectory.FullName, DirectoryChangeType.Delete);
        return AddDirectoryChange(change);
    }

    #endregion

}
