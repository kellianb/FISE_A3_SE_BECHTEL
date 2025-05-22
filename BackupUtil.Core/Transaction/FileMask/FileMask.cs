using BackupUtil.Core.Transaction.FileMask.Strategy;

namespace BackupUtil.Core.Transaction.FileMask;

/// <summary>
///     Set rules for files in a backup transaction.
/// </summary>
public class FileMask
{
    private readonly Dictionary<FileMaskEffect, List<FileMaskingStrategy>> _masks;

    public FileMask()
    {
        _masks = new Dictionary<FileMaskEffect, List<FileMaskingStrategy>>();

        foreach (FileMaskEffect value in Enum.GetValues<FileMaskEffect>())
        {
            _masks.Add(value, []);
        }
    }

    internal void AddMask(FileMaskEffect effect, FileMaskingStrategy strategy)
    {
        _masks[effect].Add(strategy);
    }

    private bool ApplyMask(FileMaskEffect effect, FileInfo file)
    {
        // Only apply masks for the given effect
        return _masks[effect]
            // All strategies must return true for the file to be allowed
            .All(strategy => strategy.IsOk(file));
    }

    /// <summary>
    ///     Determine whether a file should be copied
    /// </summary>
    /// <param name="file">File to check</param>
    /// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
    public bool ShouldCopy(FileInfo file)
    {
        return ApplyMask(FileMaskEffect.Copy, file);
    }

    /// <summary>
    ///     Determine whether a file should be encrypted
    /// </summary>
    /// <param name="file">File to check</param>
    /// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
    public bool ShouldEncrypt(FileInfo file)
    {
        return ApplyMask(FileMaskEffect.Encrypt, file);
    }
}

/// <summary>
///     Effect of a file mask.
///     If no associated strategies return false, the effect is applied.
/// </summary>
public enum FileMaskEffect
{
    Copy,
    Encrypt
}
