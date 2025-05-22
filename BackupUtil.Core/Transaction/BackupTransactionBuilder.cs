using BackupUtil.Core.Transaction.Compare;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction;

internal class BackupTransactionBuilder(FileCompare fileCompare) : IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job, FileMask.FileMask fileMask)
    {
        return AddJobToTransaction(job, new BackupTransaction(fileMask));
    }

    public BackupTransaction Build(List<Job.Job> job, FileMask.FileMask fileMask)
    {
        return job.Aggregate(new BackupTransaction(fileMask), (acc, next) => AddJobToTransaction(next, acc));
    }

    private BackupTransaction AddJobToTransaction(Job.Job job, BackupTransaction transaction)
    {
        try
        {
            job.SourcePath = Path.GetFullPath(job.SourcePath);
        }
        catch (Exception e)
        {
            throw new ArgumentException("errorSourcePath", e);
        }

        try
        {
            job.TargetPath = Path.GetFullPath(job.TargetPath);
        }
        catch (Exception e)
        {
            throw new ArgumentException("errorTargetPath", e);
        }

        if (job.SourcePath == job.TargetPath)
        {
            throw new ArgumentException("errorSameSourceTarget");
        }

        if (job.TargetPath.StartsWith(job.SourcePath))
        {
            throw new ArgumentException("errorTargetInSource");
        }

        if (job.SourcePath.StartsWith(job.TargetPath))
        {
            throw new ArgumentException("errorSourceInTarget");
        }

        if (File.Exists(job.SourcePath))
        {
            if (job.TargetPath.EndsWith('\\'))
            {
                throw new ArgumentException("errorTargetPathEnd");
            }

            if (Directory.Exists(job.TargetPath))
            {
                throw new ArgumentException("errorSourceFileTargetDir");
            }

            return new SingleFileCompare(
                    new FileInfo(job.SourcePath),
                    job.TargetPath,
                    job.Differential,
                    fileCompare,
                    job.EncryptionKey)
                .Compare(transaction);
        }

        if (Directory.Exists(job.SourcePath))
        {
            if (File.Exists(job.TargetPath))
            {
                throw new ArgumentException("errorSourceDirTargetFile");
            }

            return new DirectoryCompare(
                    new DirectoryInfo(job.SourcePath),
                    job.TargetPath,
                    job.Recursive,
                    job.Differential,
                    fileCompare,
                    job.EncryptionKey)
                .Compare(transaction);
        }

        throw new FileNotFoundException("errorSourceNotFound");
    }
}
