namespace BackupUtil.Core.Encryptor;

internal interface IEncryptor
{
    internal string Encrypt(string value, string key);
}
