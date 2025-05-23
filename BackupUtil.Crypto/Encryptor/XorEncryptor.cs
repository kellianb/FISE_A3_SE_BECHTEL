namespace BackupUtil.Crypto.Encryptor;

internal class XorEncryptor : IEncryptor
{
    private readonly string _key;

    public XorEncryptor(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("errorEncryptionKeyEmpty");
        }

        _key = key;
    }

    public string Encrypt(string input)
    {
        return new string(input.Select((c, i) =>
        {
            uint charCode = c;

            uint keyCode = _key[i % _key.Length];

            return (char)(charCode ^ keyCode);
        }).ToArray());
    }
}
