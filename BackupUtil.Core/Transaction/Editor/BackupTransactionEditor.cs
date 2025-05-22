using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Transaction.FileMask;

namespace BackupUtil.Core.Transaction.Editor;

internal class BackupTransactionEditor
{
    private readonly BackupTransaction _transaction;
    private FileMask.FileMask _fileMask;

    private BackupTransactionEditor(BackupTransaction? transaction = null, FileMask.FileMask? fileMask = null)
    {
        _fileMask = fileMask ?? FileMaskBuilder.Empty();
        _transaction = transaction ?? new BackupTransaction();
    }

    public static BackupTransactionEditor New()
    {
        return new BackupTransactionEditor();
    }

    public static BackupTransactionEditor WithMask(FileMask.FileMask fileMask)
    {
        return new BackupTransactionEditor(fileMask: fileMask);
    }

    public static BackupTransactionEditor FromTransaction(BackupTransaction transaction, FileMask.FileMask fileMask)
    {
        return new BackupTransactionEditor(transaction, fileMask);
    }

    public BackupTransactionEditor SetMask(FileMask.FileMask mask)
    {
        _fileMask = mask;
        return this;
    }

    public BackupTransaction Get()
    {
        return _transaction;
    }

    #region Add directory Changes to transaction

    public BackupTransactionEditor AddDirectoryCreation(string path)
    {
        _transaction.AddDirectoryChange(DirectoryChange.Creation(path));
        return this;
    }

    public BackupTransactionEditor AddDirectoryDeletion(DirectoryInfo targetDirectory)
    {
        _transaction.AddDirectoryChange(DirectoryChange.Deletion(targetDirectory.FullName));
        return this;
    }

    #endregion


    #region Add file Changes

    public BackupTransactionEditor AddFileCreation(FileInfo sourceFile, string targetFilePath, string? encryptionKey)
    {
        if (!_fileMask.ShouldCopy(sourceFile))
        {
            return this;
        }

        if (encryptionKey != null && !_fileMask.ShouldEncrypt(sourceFile))
        {
            encryptionKey = null;
        }

        FileChange change = FileChange.Creation(sourceFile.FullName,
            targetFilePath,
            sourceFile.Length,
            encryptionKey);

        _transaction.AddFileChange(change);

        return this;
    }

    public BackupTransactionEditor AddFileUpdate(FileInfo sourceFile, FileInfo targetFile, string? encryptionKey)
    {
        if (!_fileMask.ShouldCopy(sourceFile))
        {
            return this;
        }

        if (encryptionKey != null && !_fileMask.ShouldEncrypt(sourceFile))
        {
            encryptionKey = null;
        }

        FileChange change = FileChange.Modification(sourceFile.FullName,
            targetFile.FullName,
            sourceFile.Length,
            encryptionKey);

        _transaction.AddFileChange(change);

        return this;
    }

    public BackupTransactionEditor AddFileDeletion(FileInfo targetFile)
    {
        FileChange change = FileChange.Deletion(targetFile.FullName);

        _transaction.AddFileChange(change);

        return this;
    }

    #endregion
}
