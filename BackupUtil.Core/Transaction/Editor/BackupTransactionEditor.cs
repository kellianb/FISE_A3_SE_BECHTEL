using BackupUtil.Core.Transaction.ChangeType;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Crypto.Encryptor;

namespace BackupUtil.Core.Transaction.Editor;

internal class BackupTransactionEditor
{
    private readonly BackupTransaction _transaction;
    private IEncryptor? _encryptor;
    private FileMask.FileMask _fileMask;

    private BackupTransactionEditor(
        BackupTransaction? transaction = null,
        FileMask.FileMask? fileMask = null,
        IEncryptor? encryptor = null)
    {
        _fileMask = fileMask ?? FileMaskBuilder.Empty();
        _transaction = transaction ?? new BackupTransaction();
        _encryptor = encryptor;
    }

    public static BackupTransactionEditor New()
    {
        return new BackupTransactionEditor();
    }

    public static BackupTransactionEditor WithMaskAndEncryptor(FileMask.FileMask fileMask,
        IEncryptor? encryptor = null)
    {
        return new BackupTransactionEditor(fileMask: fileMask, encryptor: encryptor);
    }

    public static BackupTransactionEditor FromTransaction(BackupTransaction transaction, FileMask.FileMask fileMask,
        IEncryptor encryptor)
    {
        return new BackupTransactionEditor(transaction, fileMask, encryptor);
    }

    public BackupTransactionEditor SetMask(FileMask.FileMask mask)
    {
        _fileMask = mask;
        return this;
    }

    public BackupTransactionEditor SetEncryptor(IEncryptor? encryptor)
    {
        _encryptor = encryptor;
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

    public BackupTransactionEditor AddFileCreation(FileInfo sourceFile, string targetFilePath)
    {
        FileMaskEffect effects = _fileMask.GetEffects(sourceFile);

        if (!effects.HasFlag(FileMaskEffect.Copy))
        {
            return this;
        }

        // Only encrypt if the file should be encrypted and the encryptor is not null
        IEncryptor? encryptor = _encryptor is null || effects.HasFlag(FileMaskEffect.Encrypt)
            ? _encryptor
            : null;

        FileChange change = FileChange.Creation(sourceFile.FullName,
            targetFilePath,
            sourceFile.Length,
            encryptor,
            effects);

        _transaction.AddFileChange(change);

        return this;
    }

    public BackupTransactionEditor AddFileUpdate(FileInfo sourceFile, FileInfo targetFile)
    {
        FileMaskEffect effects = _fileMask.GetEffects(sourceFile);

        if (!effects.HasFlag(FileMaskEffect.Copy))
        {
            return this;
        }

        // Only encrypt if the file should be encrypted and the encryptor is not null
        IEncryptor? encryptor = _encryptor is null || effects.HasFlag(FileMaskEffect.Encrypt)
            ? _encryptor
            : null;

        FileChange change = FileChange.Modification(sourceFile.FullName,
            targetFile.FullName,
            sourceFile.Length,
            encryptor,
            effects);

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
