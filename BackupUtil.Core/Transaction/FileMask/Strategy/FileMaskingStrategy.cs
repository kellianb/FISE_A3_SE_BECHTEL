using System.Text.Json.Serialization;

namespace BackupUtil.Core.Transaction.FileMask.Strategy;

[JsonDerivedType(typeof(FileMaskingStrategy), "base")]
[JsonDerivedType(typeof(MaxFileSizeStrategy), "MaxSize")]
[JsonDerivedType(typeof(MinFileSizeStrategy), "MinSize")]
[JsonDerivedType(typeof(AllowedFileExtensionsStrategy), "AllowedExtensions")]
[JsonDerivedType(typeof(BannedFileExtensionsStrategy), "BannedExtensions")]
internal class FileMaskingStrategy
{
    public virtual bool IsOk(FileInfo file)
    {
        return true;
    }
}
