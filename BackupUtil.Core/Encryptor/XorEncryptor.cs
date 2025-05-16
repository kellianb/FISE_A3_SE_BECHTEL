namespace BackupUtil.Core.Encryptor;

internal class XorEncryptor : IEncryptor
{
    public string Encrypt(string input, string key)
    {
        return new string(input.Select((c, i) =>
        {
            uint charCode = c;

            uint keyCode = key[i % key.Length];

            return (char)(charCode ^ keyCode);
        }).ToArray());
    }
}
