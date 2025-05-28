using System.Text.Json.Serialization;
using BackupUtil.Core.Transaction.FileMask.Strategy;

namespace BackupUtil.Core.Transaction.FileMask;

/// <summary>
///     Set rules for files in a backup transaction.
/// </summary>
internal class FileMask
{
    [JsonInclude] public readonly Dictionary<FileMaskEffect, List<FileMaskingStrategy>> Masks;

    public FileMask()
    {
        Masks = new Dictionary<FileMaskEffect, List<FileMaskingStrategy>>();
        foreach (FileMaskEffect value in Enum.GetValues<FileMaskEffect>())
        {
            Masks.Add(value, []);
        }
    }

    [JsonConstructor]
    public FileMask(Dictionary<FileMaskEffect, List<FileMaskingStrategy>> masks)
    {
        Masks = masks;
    }

    internal void AddMask(FileMaskEffect effect, FileMaskingStrategy strategy)
    {
        Masks[effect].Add(strategy);
    }

    private bool ApplyMask(FileMaskEffect effect, FileInfo file)
    {
        // Only apply masks for the given effect
        return Masks[effect]
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
    Encrypt,
    Prioritise
}
