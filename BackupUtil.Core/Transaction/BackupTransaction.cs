using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Transaction;

// Class to hold all the changes that need to be applied
internal class BackupTransaction
{
    public List<FileChange> FileChanges { get; set; } = [];
    public List<DirectoryChange> DirectoryChanges { get; set; } = [];

    public BackupTransaction AddDirectoryChange(DirectoryChange change)
    {
        DirectoryChanges.Add(change);
        return this;
    }

    public BackupTransaction AddFileChange(FileChange change)
    {
        FileChanges.Add(change);
        return this;
    }

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
}
