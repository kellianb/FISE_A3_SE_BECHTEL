using BackupUtil.Crypto;

namespace BackupUtil.ViewModel.ViewModel;

public enum EncryptionTypeOptions
{
    Xor,
    None
}

public static class EncryptionTypeOptionsUtils
{
    public static EncryptionType? To(EncryptionTypeOptions type)
    {
        return type switch
        {
            EncryptionTypeOptions.Xor => EncryptionType.Xor,
            _ => null
        };
    }

    public static EncryptionTypeOptions From(EncryptionType? type)
    {
        return type switch
        {
            EncryptionType.Xor => EncryptionTypeOptions.Xor,
            _ => EncryptionTypeOptions.None
        };
    }
}
