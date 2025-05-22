namespace BackupUtil.Core.Transaction.FileMask.Strategy;

/// <summary>
///     Filter files by banned extensions
/// </summary>
/// <param name="bannedExtensions">
///     List of banned file extensions. File extensions need to include the leading <c>.</c>
/// </param>
/// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
internal class BannedFileExtensionsStrategy(List<string> bannedExtensions) : FileMaskingStrategy
{
    /// <summary>
    ///     List of banned file extensions. File extensions need to include the leading <c>.</c>
    /// </summary>
    public List<string> BannedExtensions { get; set; } = bannedExtensions;

    public override bool IsOk(FileInfo file)
    {
        return !BannedExtensions.Contains(file.Extension);
    }
}
