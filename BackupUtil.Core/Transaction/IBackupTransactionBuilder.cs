namespace BackupUtil.Core.Transaction;

internal interface IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job);
    public BackupTransaction Build(List<Job.Job> job);
}
