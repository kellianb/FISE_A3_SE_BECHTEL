using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Transaction;

// Class to hold all the changes that need to be applied
public class BackupTransaction
{
    public List<FileChange> FileChanges { get; } = [];
    public List<DirectoryChange> DirectoryChanges { get; } = [];

    #region Get info about directory changes

    /// <summary>
    ///     Get the list of folders that will undergo a certain type of change
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    public string[] GetConcernedFolders(DirectoryChangeType changeType)
    {
        return DirectoryChanges.FindAll(x => x.ChangeType == changeType)
            .Select(x => x.TargetPath)
            .ToArray();
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

    #region Get info about file changes

    /// <summary>
    ///     Get the total size of the files that will be copied
    /// </summary>
    /// <returns></returns>
    public long GetTotalCopiedFileSize()
    {
        return FileChanges.Sum(x => x.ChangeType == FileChangeType.Delete ? 0 : x.FileSize);
    }

    /// <summary>
    ///     Get the list of files that will undergo a certain type of change
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    public string[] GetConcernedFiles(FileChangeType changeType)
    {
        return FileChanges.FindAll(x => x.ChangeType == changeType)
            .Select(x => x.TargetPath)
            .ToArray();
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
        FileChange change = new(targetFile.FullName, FileChangeType.Delete, fileSize: targetFile.Length);
        return AddFileChange(change);
    }

    #endregion
}
