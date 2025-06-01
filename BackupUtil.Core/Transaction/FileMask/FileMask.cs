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
    ///     Determine the effects which apply to a file
    /// </summary>
    /// <param name="fileInfo">File to check</param>
    /// <returns>The effects the file is subject to</returns>
    public FileMaskEffect GetEffects(FileInfo fileInfo)
    {
        FileMaskEffect combined = 0;

        foreach (FileMaskEffect value in Enum.GetValues<FileMaskEffect>())
        {
            if (ApplyMask(value, fileInfo))
            {
                combined |= value;
            }
        }

        return combined;
    }
}

/// <summary>
///     Effect of a file mask.
///     If no associated strategies return false, the effect is applied.
/// </summary>
[Flags]
public enum FileMaskEffect
{
    /// <summary>
    ///     The file will be copied.
    /// </summary>
    Copy = 1,

    /// <summary>
    ///     The file will be encrypted.
    /// </summary>
    Encrypt = 2,

    /// <summary>
    ///     Files with this effect will be copied before all other files.
    /// </summary>
    Prioritize = 4,

    /// <summary>
    ///     Files with this effect will be copied concurrently
    /// </summary>
    Parallelize = 8
}
