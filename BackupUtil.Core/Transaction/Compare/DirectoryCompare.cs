using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction.Compare;

internal class DirectoryCompare(
    DirectoryInfo sourceDirectory,
    string targetDirectoryPath,
    bool recursive,
    bool differential)
    : ICompare
{
    private readonly bool _differential = differential;
    private readonly bool _recursive = recursive;
    private readonly DirectoryInfo _sourceDirectory = sourceDirectory;
    private readonly string _targetDirectoryPath = targetDirectoryPath;


    public BackupTransaction Compare(BackupTransaction transaction)
    {
        return _differential
            ? Differential(transaction, _sourceDirectory, _targetDirectoryPath, _recursive)
            : Full(transaction, _sourceDirectory, _targetDirectoryPath, _recursive);
    }

    /// <summary>
    ///     Make a full backup of a directory
    /// </summary>
    private static BackupTransaction Full(BackupTransaction transaction, DirectoryInfo sourceDirectory,
        string targetDirectoryPath, bool recursive = false)
    {
        transaction.AddDirectoryCreation(targetDirectoryPath);

        // Copy all files
        transaction = FullDirectoryFiles(sourceDirectory, targetDirectoryPath, transaction);

        if (!recursive)
        {
            return transaction;
        }

        // Create all subdirectories
        foreach (DirectoryInfo sourceSubDirectory in sourceDirectory.GetDirectories())
        {
            string targetSubDirectoryPath = Path.Combine(targetDirectoryPath, sourceSubDirectory.Name);

            transaction = Full(transaction, sourceSubDirectory, targetSubDirectoryPath, recursive);
        }

        return transaction;
    }

    /// <summary>
    ///     Make a differential backup of a directory TODO: WIP
    /// </summary>
    private static BackupTransaction Differential(BackupTransaction transaction, DirectoryInfo sourceDirectory,
        string targetDirectoryPath, bool recursive = false)
    {
        // Check if the directory exists
        if (Directory.Exists(targetDirectoryPath))
        {
            // If it exists, check for and delete removed directories
            DirectoryInfo targetDirectory = new(targetDirectoryPath);

            foreach (DirectoryInfo dir in targetDirectory.GetDirectories()
                         .ExceptBy(sourceDirectory.GetDirectories().Select(d => d.Name),
                             d => d.Name))
            {
                transaction.AddDirectoryDeletion(dir);
            }

            // Diff directory files
            transaction = DiffDirectoryFiles(sourceDirectory, targetDirectory, transaction);
        }
        else
        {
            // If it does not exist, create it
            transaction.AddDirectoryCreation(targetDirectoryPath);

            transaction = FullDirectoryFiles(sourceDirectory, targetDirectoryPath, transaction);
        }

        // Handle subdirectories
        foreach (DirectoryInfo sourceSubDirectory in sourceDirectory.GetDirectories())
        {
            string targetSubDirectoryPath = Path.Combine(targetDirectoryPath, sourceSubDirectory.Name);

            if (recursive)
            {
                transaction = Differential(transaction, sourceSubDirectory, targetSubDirectoryPath, recursive);
            }
            else if (!Directory.Exists(targetSubDirectoryPath))
            {
                transaction.AddDirectoryCreation(targetSubDirectoryPath);
            }
        }

        return transaction;
    }

    /// <summary>
    ///     Make a differential backup of a directory's files
    /// </summary>
    private static BackupTransaction DiffDirectoryFiles(DirectoryInfo sourceDirectory,
        DirectoryInfo targetDirectory, BackupTransaction transaction)
    {
        FileInfo[] sourceFiles = sourceDirectory.GetFiles();
        FileInfo[] targetFiles = targetDirectory.GetFiles();

        // -- Get files to create --
        foreach (FileInfo fileInfo in sourceFiles.ExceptBy(targetFiles.Select(f => f.Name),
                     f => f.Name))
        {
            transaction.AddFileCreation(fileInfo, Path.Combine(targetDirectory.FullName, fileInfo.Name));
        }

        // -- Get files to update --
        IEnumerable<string> commonFileNames =
            sourceFiles.Select(f => f.Name).Intersect(targetFiles.Select(f => f.Name));

        // Create a dictionary of target files by name
        Dictionary<string, FileInfo> targetFilesByName = targetFiles.ToDictionary(f => f.Name);

        foreach (FileInfo sourceFile in sourceFiles.Where(f => commonFileNames.Contains(f.Name)))
        {
            // Get the corresponding target file
            FileInfo targetFile = targetFilesByName[sourceFile.Name];

            if (FileCompare.AreFilesEqual(sourceFile, targetFile))
            {
                continue;
            }

            transaction.AddFileUpdate(sourceFile, targetFile);
        }

        // -- Get files to delete --
        foreach (FileInfo fileInfo in targetFiles.ExceptBy(sourceFiles.Select(f => f.Name),
                     f => f.Name))
        {
            transaction.AddFileDeletion(fileInfo);
        }

        return transaction;
    }


    /// <summary>
    ///     Make a full backup of a directory's files
    /// </summary>
    private static BackupTransaction FullDirectoryFiles(DirectoryInfo sourceDirectory,
        string targetDirectoryPath, BackupTransaction transaction)
    {
        foreach (FileInfo file in sourceDirectory.GetFiles())
        {
            transaction.AddFileCreation(file, Path.Combine(targetDirectoryPath, file.Name));
        }

        return transaction;
    }
}
