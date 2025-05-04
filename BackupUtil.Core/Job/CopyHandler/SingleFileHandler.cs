namespace BackupUtil.Core.Job.CopyHandler;

public class SingleFileHandler : ICopyHandler
{
    private readonly FileInfo _sourceFile;
    private readonly string _targetFilePath;

    public SingleFileHandler(FileInfo sourceFile, string targetFilePath)
    {
        if (targetFilePath.EndsWith('\\'))
        {
            throw new ArgumentException("Target file path must not end with a '\\'");
        }

        _targetFilePath = targetFilePath;
        _sourceFile = sourceFile;
    }

    public void Run()
    {
        _sourceFile.CopyTo(_targetFilePath, true);
    }
}
