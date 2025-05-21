using System.Security.Cryptography;
using System.Text;
using BackupUtil.Core.Encryptor;

namespace BackupUtil.Core.Util;

internal class FileCompare(IEncryptor encryptor)
{
    /// <summary>
    ///     Compare an unencrypted file with an encrypted file with the same key
    /// </summary>
    /// <param name="file1">FileInfo of the unencrypted file</param>
    /// <param name="file2">FileInfo of the encrypted file</param>
    /// <param name="key">Key to encrypt file1 with</param>
    /// <returns>Whether the two files are equal</returns>
    public bool AreFilesEqual(FileInfo file1, FileInfo file2, string? key)
    {
        string hash1;
        if (key is null)
        {
            if (file1.Length != file2.Length)
            {
                return false;
            }

            using FileStream stream = file1.OpenRead();
            hash1 = CalculateFileHash(stream);
        }
        else
        {
            // Encrypt file1 with the key before hashing it
            string encryptedFile1;
            using (StreamReader reader = file1.OpenText())
            {
                encryptedFile1 = encryptor.Encrypt(reader.ReadToEnd(), key);
            }

            using (MemoryStream stream = new(Encoding.UTF8.GetBytes(encryptedFile1)))
            {
                hash1 = CalculateFileHash(stream);
            }
        }

        string hash2 = CalculateFileHash(file2.OpenRead());

        return hash1.Equals(hash2, StringComparison.OrdinalIgnoreCase);
    }

    private static string CalculateFileHash(Stream stream)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes);
    }
}
