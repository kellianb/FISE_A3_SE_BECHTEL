using System.Text.Json;
using BackupUtil.Core.Transaction.FileMask.Strategy;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction.FileMask;

public class FileMaskBuilder
{
    private readonly FileMask _fileMask;

    private FileMaskBuilder(FileMask? mask = null)
    {
        _fileMask = mask ?? new FileMask();
    }

    internal static FileMask Empty()
    {
        return new FileMask();
    }

    public static FileMaskBuilder New()
    {
        return new FileMaskBuilder();
    }

    internal static FileMaskBuilder From(FileMask mask)
    {
        return new FileMaskBuilder(mask);
    }

    public static FileMaskBuilder FromString(string serialized)
    {
        return new FileMaskBuilder(JsonSerializer.Deserialize<FileMask>(serialized, JsonBackupUtilSerializerContext.Default.FileMask));
    }

    internal FileMask Build()
    {
        return _fileMask;
    }

    public string BuildSerialized()
    {
        return JsonSerializer.Serialize(Build());
    }

    /// <summary>
    ///     Set a maximum file size for the given effect.
    /// </summary>
    /// <param name="maxSize">Maximum allowed file size</param>
    /// <param name="effect">Operation that will only be applied to matching files</param>
    public FileMaskBuilder MaxFileSize(long maxSize, FileMaskEffect effect)
    {
        _fileMask.AddMask(effect, new MaxFileSizeStrategy(maxSize));
        return this;
    }

    /// <summary>
    ///     Set a minimum file size for the given effect.
    /// </summary>
    /// <param name="minSize">Minimum allowed file size</param>
    /// <param name="effect">Operation that will only be applied to matching files</param>
    public FileMaskBuilder MinFileSize(long minSize, FileMaskEffect effect)
    {
        _fileMask.AddMask(effect, new MinFileSizeStrategy(minSize));
        return this;
    }

    /// <summary>
    ///     Set a list of allowed file extensions for the given effect.
    /// </summary>
    /// <param name="allowedExtensions">List of allowed file extensions</param>
    /// <param name="effect">Operation that will only be applied to matching files</param>
    public FileMaskBuilder AllowedExtensions(List<string> allowedExtensions, FileMaskEffect effect)
    {
        _fileMask.AddMask(effect, new AllowedFileExtensionsStrategy(allowedExtensions));
        return this;
    }

    /// <summary>
    ///     Set a list of banned file extensions for the given effect.
    /// </summary>
    /// <param name="bannedExtensions">List of banned file extensions</param>
    /// <param name="effect">Operation that will only be applied to matching files</param>
    public FileMaskBuilder BannedExtensions(List<string> bannedExtensions, FileMaskEffect effect)
    {
        _fileMask.AddMask(effect, new BannedFileExtensionsStrategy(bannedExtensions));
        return this;
    }
}
