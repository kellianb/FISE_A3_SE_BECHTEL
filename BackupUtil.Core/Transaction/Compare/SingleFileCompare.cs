using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction.Compare;

public class SingleFileCompare(FileInfo sourceFile, string targetFilePath, bool differential)
    : ICompare
{
    private readonly bool _differential = differential;
    private readonly FileInfo _sourceFile = sourceFile;
    private readonly string _targetFilePath = targetFilePath;


    public BackupTransaction Compare()
    {
        return _differential ? Differential() : Full();
    }

    private BackupTransaction Differential()
    {
        BackupTransaction diff = new();

        // If the target file already exists, check if it needs to be updated
        if (File.Exists(_targetFilePath))
        {
            FileInfo targetFile = new(_targetFilePath);

            return FileCompare.AreFilesEqual(_sourceFile, targetFile)
                ? diff
                : diff.AddFileUpdate(_sourceFile, targetFile);
        }

        // Create the file
        diff.AddFileCreation(_sourceFile, _targetFilePath);

        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);

        // If the target directory is the filesystem root or exists, return the diff
        if (string.IsNullOrEmpty(targetDirectoryName) || Directory.Exists(targetDirectoryName))
        {
            return diff;
        }

        // Else create the target directory
        return diff.AddDirectoryCreation(targetDirectoryName);
    }

    private BackupTransaction Full()
    {
        BackupTransaction full = new();

        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);
        if (!string.IsNullOrEmpty(targetDirectoryName))
        {
            full.AddDirectoryCreation(targetDirectoryName);
        }

        return full.AddFileCreation(_sourceFile, _targetFilePath);
    }
}
