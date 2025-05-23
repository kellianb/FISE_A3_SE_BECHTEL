using BackupUtil.Crypto.Encryptor;

namespace BackupUtil.Crypto;

public static class EncryptorBuilder
{
    public static IEncryptor New(EncryptionType type, string key)
    {
        return type switch
        {
            EncryptionType.Xor => new XorEncryptor(key),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}

public enum EncryptionType
{
    Xor
}
