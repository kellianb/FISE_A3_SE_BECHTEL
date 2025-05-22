using BackupUtil.Core.Transaction.FileMask.Strategy;

namespace BackupUtil.Core.Transaction.FileMask;

public class FileMaskBuilder
{
    private readonly FileMask _fileMask = new();

    public FileMask Build()
    {
        return _fileMask;
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
