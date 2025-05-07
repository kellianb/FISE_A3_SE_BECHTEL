using BackupUtil.Core.Transaction.Compare;

namespace BackupUtil.Core.Transaction;

public static class BackupTransactionBuilder
{
    public static BackupTransaction Build(Job.Job job)
    {
        try
        {
            job.SourcePath = Path.GetFullPath(job.SourcePath);
        }
        catch (Exception e)
        {
            throw new ArgumentException("Source path is not valid", e);
        }

        try
        {
            job.TargetPath = Path.GetFullPath(job.TargetPath);
        }
        catch (Exception e)
        {
            throw new ArgumentException("Target path is not valid", e);
        }

        if (job.SourcePath == job.TargetPath)
        {
            throw new ArgumentException("Source and target paths must not be the same");
        }

        if (job.TargetPath.StartsWith(job.SourcePath))
        {
            throw new ArgumentException("Target path must not be a subdirectory of source path");
        }

        if (job.SourcePath.StartsWith(job.TargetPath))
        {
            throw new ArgumentException("Source path must not be a subdirectory of target path");
        }

        if (File.Exists(job.SourcePath))
        {
            if (job.TargetPath.EndsWith('\\'))
            {
                throw new ArgumentException("Target file path must not end with a '\\'");
            }

            if (Directory.Exists(job.TargetPath))
            {
                throw new ArgumentException("Source path is a file, target path must not be a directory");
            }

            return new SingleFileCompare(new FileInfo(job.SourcePath), job.TargetPath, job.Differential).Compare();
        }

        if (Directory.Exists(job.SourcePath))
        {
            if (File.Exists(job.TargetPath))
            {
                throw new ArgumentException("Source path is a directory, target path must not be a file");
            }

            return new DirectoryCompare(new DirectoryInfo(job.SourcePath), job.TargetPath, job.Recursive,
                job.Differential).Compare();
        }

        throw new FileNotFoundException("Source file or directory not found");
    }
}
