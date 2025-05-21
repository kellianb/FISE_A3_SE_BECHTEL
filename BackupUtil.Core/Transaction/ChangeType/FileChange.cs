using BackupUtil.I18n;

namespace BackupUtil.Core.Transaction.ChangeType;

public enum FileChangeType
{
    [I18NKey("changeTypeCreate")] Create = 0,
    [I18NKey("changeTypeModify")] Modify = 1,
    [I18NKey("changeTypeDelete")] Delete = 2
}

//
/// <summary>
///     Represents a change made to a file, containing details such as the type of change,
///     file size, and optional source path and encryption key.
///     This class is used to describe file-level changes that occur within a transaction
///     during backup operations, such as creation, modification, or deletion of a file.
/// </summary>
internal class FileChange : FileSystemChange,
    IEquatable<FileChange>
{
    /// <summary>
    ///     Represents a change made to a file, containing details such as the type of change,
    ///     file size, and optional source path and encryption key.
    ///     This class is used to describe file-level changes that occur within a transaction
    ///     during backup operations, such as creation, modification, or deletion of a file.
    ///     <param name="targetPath">Path of the file to apply changes to</param>
    ///     <param name="changeType">Type of change to make to the file at targetPath</param>
    ///     <param name="sourcePath">Path of the source file, only set for Creation and Modifications</param>
    ///     <param name="fileSize">Size of the source file, only set for Creation and Modifications</param>
    ///     <param name="encryptionKey">Key to encrypt the file with, leave null if no encryption should be applied</param>
    /// </summary>
    private FileChange(string targetPath,
        FileChangeType changeType,
        string? sourcePath = null,
        long fileSize = 0,
        string? encryptionKey = null) : base(targetPath)
    {
        ChangeType = changeType;
        FileSize = fileSize;
        SourcePath = sourcePath;
        EncryptionKey = encryptionKey;
    }

    public FileChangeType ChangeType { get; }
    public long FileSize { get; }
    public string? SourcePath { get; }

    public string? EncryptionKey { get; }

    public static FileChange Creation(string sourcePath, string targetPath, long fileSize, string? encryptionKey = null)
    {
        return new FileChange(targetPath, FileChangeType.Create, sourcePath, fileSize, encryptionKey);
    }

    public static FileChange Modification(string sourcePath, string targetPath, long fileSize,
        string? encryptionKey = null)
    {
        return new FileChange(targetPath, FileChangeType.Modify, sourcePath, fileSize, encryptionKey);
    }

    public static FileChange Deletion(string targetPath)
    {
        return new FileChange(targetPath, FileChangeType.Delete);
    }

    #region Equality

    public bool Equals(FileChange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ChangeType == other.ChangeType && FileSize == other.FileSize && SourcePath == other.SourcePath;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((FileChange)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)ChangeType, FileSize, SourcePath);
    }

    #endregion
}
