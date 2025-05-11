using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction.Compare;

public class SingleFileCompare(FileInfo sourceFile, string targetFilePath, bool differential)
    : ICompare
{
    private readonly bool _differential = differential;
    private readonly FileInfo _sourceFile = sourceFile;
    private readonly string _targetFilePath = targetFilePath;


    public BackupTransaction Compare(BackupTransaction transaction)
    {
        return _differential ? Differential(transaction) : Full(transaction);
    }

    private BackupTransaction Differential(BackupTransaction transaction)
    {
        // If the target file already exists, check if it needs to be updated
        if (File.Exists(_targetFilePath))
        {
            FileInfo targetFile = new(_targetFilePath);

            return FileCompare.AreFilesEqual(_sourceFile, targetFile)
                ? transaction
                : transaction.AddFileUpdate(_sourceFile, targetFile);
        }

        // Create the file
        transaction.AddFileCreation(_sourceFile, _targetFilePath);

        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);

        // If the target directory is the filesystem root or exists, return the diff
        if (string.IsNullOrEmpty(targetDirectoryName) || Directory.Exists(targetDirectoryName))
        {
            return transaction;
        }

        // Else create the target directory
        return transaction.AddDirectoryCreation(targetDirectoryName);
    }

    private BackupTransaction Full(BackupTransaction transaction)
    {
        string? targetDirectoryName = Path.GetDirectoryName(_targetFilePath);
        if (!string.IsNullOrEmpty(targetDirectoryName))
        {
            transaction.AddDirectoryCreation(targetDirectoryName);
        }

        return transaction.AddFileCreation(_sourceFile, _targetFilePath);
    }
}
