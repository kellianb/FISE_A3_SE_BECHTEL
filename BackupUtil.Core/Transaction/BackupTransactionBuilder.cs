using BackupUtil.Core.Transaction.Compare;
using BackupUtil.Core.Transaction.Editor;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Transaction;

internal class BackupTransactionBuilder(FileCompare fileCompare) : IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job)
    {
        FileMask.FileMask mask = string.IsNullOrWhiteSpace(job.FileMask)
            ? FileMaskBuilder.Empty()
            : FileMaskBuilder.FromString(job.FileMask).Build();

        return AddJobToTransaction(job, BackupTransactionEditor.WithMask(mask)).Get();
    }

    public BackupTransaction Build(List<Job.Job> jobs)
    {
        return jobs.Aggregate(BackupTransactionEditor.New(), (acc, next) =>
        {
            FileMask.FileMask mask = string.IsNullOrWhiteSpace(next.FileMask)
                ? FileMaskBuilder.Empty()
                : FileMaskBuilder.FromString(next.FileMask).Build();

            return AddJobToTransaction(next, acc.SetMask(mask));
        }).Get();
    }

    private BackupTransactionEditor AddJobToTransaction(Job.Job job, BackupTransactionEditor editor)
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
                .Compare(editor);
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
                .Compare(editor);
        }

        throw new FileNotFoundException("errorSourceNotFound");
    }
}
