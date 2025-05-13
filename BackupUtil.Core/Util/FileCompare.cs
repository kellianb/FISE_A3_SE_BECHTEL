using System.Security.Cryptography;

namespace BackupUtil.Core.Util;

internal static class FileCompare
{
    public static bool AreFilesEqual(FileInfo file1, FileInfo file2)
    {
        if (file1.Length != file2.Length)
        {
            return false;
        }

        // Compare hashes
        string hash1 = CalculateFileHash(file1);
        string hash2 = CalculateFileHash(file2);

        return hash1.Equals(hash2, StringComparison.OrdinalIgnoreCase);
    }

    private static string CalculateFileHash(FileInfo file)
    {
        using SHA256 sha256 = SHA256.Create();
        using FileStream stream = file.OpenRead();
        byte[] hashBytes = sha256.ComputeHash(stream);
        return Convert.ToHexString(hashBytes);
    }
}
