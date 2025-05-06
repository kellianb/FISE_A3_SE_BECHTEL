using BackupUtil.Core.Transaction.Compare;

namespace BackupUtil.Core.Transaction;

public static class BackupTransactionBuilder
{
    public static BackupTransaction Build(Job.Job job)
    {
        if (File.Exists(job.SourcePath))
        {
            if (job.TargetPath.EndsWith('\\'))
            {
                throw new ArgumentException("Target file path must not end with a '\\'");
            }

            return new SingleFileCompare(new FileInfo(job.SourcePath), job.TargetPath, job.Differential).Compare();
        }

        if (Directory.Exists(job.SourcePath))
        {
            return new DirectoryCompare(new DirectoryInfo(job.SourcePath), job.TargetPath, job.Recursive,
                job.Differential).Compare();
        }

        throw new FileNotFoundException("Source file or directory not found");
    }
}
