using BackupUtil.Core.Transaction.Editor;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction.Compare;

internal class SingleFileCompare(
    FileInfo sourceFile,
    string targetFilePath,
    bool differential,
    FileCompare compare)
    : ICompare
{
    public BackupTransactionEditor Compare(BackupTransactionEditor transaction)
    {
        return differential ? Differential(transaction) : Full(transaction);
    }

    private BackupTransactionEditor Differential(BackupTransactionEditor transaction)
    {
        // If the target file already exists, check if it needs to be updated
        if (File.Exists(targetFilePath))
        {
            FileInfo targetFile = new(targetFilePath);

            return compare.AreFilesEqual(sourceFile, targetFile)
                ? transaction
                : transaction.AddFileUpdate(sourceFile, targetFile);
        }

        // Create the file
        transaction.AddFileCreation(sourceFile, targetFilePath);

        string? targetDirectoryName = Path.GetDirectoryName(targetFilePath);

        // If the target directory is the filesystem root or exists, return the diff
        if (string.IsNullOrEmpty(targetDirectoryName) || Directory.Exists(targetDirectoryName))
        {
            return transaction;
        }

        // Else create the target directory
        return transaction.AddDirectoryCreation(targetDirectoryName);
    }

    private BackupTransactionEditor Full(BackupTransactionEditor transaction)
    {
        string? targetDirectoryName = Path.GetDirectoryName(targetFilePath);
        if (!string.IsNullOrEmpty(targetDirectoryName))
        {
            transaction.AddDirectoryCreation(targetDirectoryName);
        }

        return transaction.AddFileCreation(sourceFile, targetFilePath);
    }
}
