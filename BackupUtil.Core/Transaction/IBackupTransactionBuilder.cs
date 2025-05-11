namespace BackupUtil.Core.Transaction;

public interface IBackupTransactionBuilder
{
    public static abstract BackupTransaction Build(Job.Job job);
    public static abstract BackupTransaction Build(List<Job.Job> job);
}
