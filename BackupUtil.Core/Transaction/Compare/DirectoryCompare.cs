using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Transaction.Compare;

public class DirectoryCompare(DirectoryInfo sourceDirectory, string targetDirectoryPath, bool recursive)
    : ICompare
{
    private readonly bool _recursive = recursive;
    private readonly DirectoryInfo _sourceDirectory = sourceDirectory;
    private readonly string _targetDirectoryPath = targetDirectoryPath;


    public BackupTransaction Compare()
    {
        throw new NotImplementedException();
    }
}
