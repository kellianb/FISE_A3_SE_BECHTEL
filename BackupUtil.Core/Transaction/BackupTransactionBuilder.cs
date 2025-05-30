using BackupUtil.Core.Transaction.Compare;
using BackupUtil.Core.Transaction.Editor;
using BackupUtil.Core.Transaction.FileMask;
using BackupUtil.Core.Util;
using BackupUtil.Crypto;
using BackupUtil.Crypto.Encryptor;

namespace BackupUtil.Core.Transaction;

internal class BackupTransactionBuilder : IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job)
    {
        FileMask.FileMask mask = string.IsNullOrWhiteSpace(job.FileMask)
            ? FileMaskBuilder.Empty()
            : FileMaskBuilder.FromString(job.FileMask).Build();

        IEncryptor? encryptor = job.EncryptionType == null
            ? null
            : EncryptorBuilder.New(job.EncryptionType.Value, job.EncryptionKey!);

        FileCompare fileCompare = new(encryptor);

        return AddJobToTransaction(job, BackupTransactionEditor.WithMaskAndEncryptor(mask, encryptor), fileCompare)
            .Get();
    }

    public BackupTransaction Build(List<Job.Job> jobs)
    {
        return jobs.Aggregate(BackupTransactionEditor.New(), (acc, next) =>
        {
            FileMask.FileMask mask = string.IsNullOrWhiteSpace(next.FileMask)
                ? FileMaskBuilder.Empty()
                : FileMaskBuilder.FromString(next.FileMask).Build();

            IEncryptor? encryptor = next.EncryptionType == null
                ? null
                : EncryptorBuilder.New(next.EncryptionType.Value, next.EncryptionKey!);

            return AddJobToTransaction(next, acc.SetMask(mask).SetEncryptor(encryptor), new FileCompare(encryptor));
        }).Get();
    }

    private BackupTransactionEditor AddJobToTransaction(Job.Job job, BackupTransactionEditor editor,
        FileCompare fileCompare)
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

        if (job.TargetPath.StartsWith(job.SourcePath + Path.DirectorySeparatorChar))
        {
            throw new ArgumentException("errorTargetInSource");
        }

        if (job.SourcePath.StartsWith(job.TargetPath + Path.DirectorySeparatorChar))
        {
            throw new ArgumentException("errorSourceInTarget");
        }

        if (File.Exists(job.SourcePath))
        {
            if (job.TargetPath.EndsWith(Path.DirectorySeparatorChar))
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
                    fileCompare)
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
                    fileCompare)
                .Compare(editor);
        }

        throw new FileNotFoundException("errorSourceNotFound");
    }
}
