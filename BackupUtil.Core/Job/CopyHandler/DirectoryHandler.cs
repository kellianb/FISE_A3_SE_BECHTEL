namespace BackupUtil.Core.Job.CopyHandler;

public class DirectoryHandler : ICopyHandler
{
    private readonly bool _recursive;
    private readonly DirectoryInfo _sourceDirectory;
    private readonly string _targetDirectoryPath;


    public DirectoryHandler(DirectoryInfo sourceDirectory, string targetDirectoryPath, bool? recursive)
    {
        _sourceDirectory = sourceDirectory;
        _targetDirectoryPath = targetDirectoryPath;
        _recursive = recursive ?? false;
    }

    public void Run()
    {
        DirectoryInfo targetDirectory = Directory.Exists(_targetDirectoryPath)
            ? new DirectoryInfo(_targetDirectoryPath)
            : Directory.CreateDirectory(_targetDirectoryPath);

        if (_recursive)
        {
            CopyDirectoryRecursively(_sourceDirectory, targetDirectory);
        }
        else
        {
            CopyDirectory(_sourceDirectory, targetDirectory);
        }
    }

    private static void CopyDirectoryRecursively(DirectoryInfo source, DirectoryInfo target)
    {
        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            CopyDirectoryRecursively(dir, target.CreateSubdirectory(dir.Name));
        }

        foreach (FileInfo file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }

    private static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
    {
        foreach (FileInfo file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}
