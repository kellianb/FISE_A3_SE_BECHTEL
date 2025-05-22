namespace BackupUtil.Core.Transaction.FileMask.Strategy;

/// <summary>
///     Filter files by minimum size
/// </summary>
/// <param name="minSize">Minimum allowed file size</param>
/// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
internal class MinFileSizeStrategy(long minSize) : FileMaskingStrategy
{
    /// <summary>Minimum allowed file size</summary>
    public long MinSize { get; set; } = minSize;

    public override bool IsOk(FileInfo file)
    {
        return file.Length >= MinSize;
    }
}
