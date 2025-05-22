namespace BackupUtil.Core.Transaction;

internal interface IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job, FileMask.FileMask fileMask);
    public BackupTransaction Build(List<Job.Job> job, FileMask.FileMask fileMask);
}
