namespace BackupUtil.Core.Transaction;

public interface IBackupTransactionBuilder
{
    public BackupTransaction Build(Job.Job job);
    public BackupTransaction Build(List<Job.Job> job);
}
