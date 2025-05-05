using BackupUtil.Core.Operation.ChangeType;

namespace BackupUtil.Core.Operation.Compare;

public class DirectoryCompare(DirectoryInfo sourceDirectory, string targetDirectoryPath, bool recursive)
    : ICompare
{
    private readonly bool _recursive = recursive;
    private readonly DirectoryInfo _sourceDirectory = sourceDirectory;
    private readonly string _targetDirectoryPath = targetDirectoryPath;


    public Transaction Compare()
    {
        throw new NotImplementedException();
    }
}
