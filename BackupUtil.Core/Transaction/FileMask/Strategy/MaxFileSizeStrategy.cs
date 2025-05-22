namespace BackupUtil.Core.Transaction.FileMask.Strategy;

/// <summary>
///     Filter files by maximum size
/// </summary>
/// <param name="maxSize">Maximum allowed file size</param>
/// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
internal class MaxFileSizeStrategy(long maxSize) : FileMaskingStrategy
{
    /// <summary>Maximum allowed file size</summary>
    public long MaxSize { get; set; } = maxSize;

    public override bool IsOk(FileInfo file)
    {
        return file.Length <= MaxSize;
    }
}
