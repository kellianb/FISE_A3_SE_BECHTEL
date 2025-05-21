using BackupUtil.I18n;

namespace BackupUtil.Core.Transaction.ChangeType;

public enum DirectoryChangeType
{
    [I18NKey("changeTypeCreate")] Create = 0,
    [I18NKey("changeTypeDelete")] Delete = 1
}

/// <summary>
///     Represents a change operation for a directory, such as creation or deletion.
/// </summary>
internal class DirectoryChange : FileSystemChange,
    IEquatable<DirectoryChange>
{
    /// <summary>
    ///     Represents a change operation for a directory, such as creation or deletion.
    ///     <param name="targetPath">Path of the directory to apply changes to</param>
    ///     <param name="changeType">Type of change to apply</param>
    /// </summary>
    private DirectoryChange(string targetPath,
        DirectoryChangeType changeType) : base(targetPath)
    {
        ChangeType = changeType;
    }

    public DirectoryChangeType ChangeType { get; }

    public static DirectoryChange Creation(string targetPath)
    {
        return new DirectoryChange(targetPath, DirectoryChangeType.Create);
    }

    public static DirectoryChange Deletion(string targetPath)
    {
        return new DirectoryChange(targetPath, DirectoryChangeType.Delete);
    }


    #region Equality

    public bool Equals(DirectoryChange? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ChangeType == other.ChangeType;
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

        return Equals((DirectoryChange)obj);
    }

    public override int GetHashCode()
    {
        return (int)ChangeType;
    }

    #endregion
}
