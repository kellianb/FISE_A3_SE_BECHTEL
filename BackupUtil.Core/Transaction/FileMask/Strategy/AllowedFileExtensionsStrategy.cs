namespace BackupUtil.Core.Transaction.FileMask.Strategy;

/// <summary>
///     Filter files by allowed extensions
/// </summary>
/// <param name="allowedExtensions">
///     List of allowed file extensions. File extensions need to include the leading <c>.</c>
/// </param>
/// <returns><c>true</c> if the file is allowed, <c>false</c> otherwise</returns>
internal class AllowedFileExtensionsStrategy(List<string> allowedExtensions) : FileMaskingStrategy
{
    /// <summary>
    ///     List of allowed file extensions. File extensions need to include the leading <c>.</c>
    /// </summary>
    public List<string> AllowedExtensions { get; set; } = allowedExtensions;

    public override bool IsOk(FileInfo file)
    {
        return AllowedExtensions.Contains(file.Extension);
    }
}
