using BackupUtil.Core.Operation.ChangeType;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Operation.Compare;

public class SingleFileCompare(FileInfo sourceFile, string targetFilePath, bool differential)
    : ICompare
{
    private readonly bool _differential = differential;
    private readonly FileInfo _sourceFile = sourceFile;
    private readonly string _targetFilePath = targetFilePath;


    public Transaction Compare()
    {
        return _differential ? Differential() : Full();
    }

    private Transaction Differential()
    {
        Transaction diff = new();

        // If the target file already exists, check if it needs to be updated
        if (File.Exists(_targetFilePath))
        {
            FileInfo targetFile = new(_targetFilePath);

            return FileCompare.AreFilesEqual(_sourceFile, targetFile)
                ? diff
                : diff.AddFileUpdate(_sourceFile, targetFile);
        }

        // If the target directory doesn't exist, create it
        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);
        if (!string.IsNullOrEmpty(targetDirectoryName) && !Directory.Exists(targetDirectoryName))
        {
            diff.AddDirectoryCreation(targetDirectoryName);
        }

        // Create the file
        return diff.AddFileCreation(_sourceFile, _targetFilePath);
        ;
    }

    private Transaction Full()
    {
        Transaction full = new();


        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);
        if (!string.IsNullOrEmpty(targetDirectoryName))
        {
            full.AddDirectoryCreation(targetDirectoryName);
        }

        return full.AddFileCreation(_sourceFile, _targetFilePath);

        return full;
    }
}
