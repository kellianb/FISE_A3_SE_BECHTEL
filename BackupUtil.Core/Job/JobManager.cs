using BackupUtil.Core.Command;
using BackupUtil.Core.Executor;
using BackupUtil.Core.Job.Loader;
using BackupUtil.Core.Transaction;

namespace BackupUtil.Core.Job;

public class JobManager
{
    private readonly IBackupTransactionBuilder _transactionBuilder = new BackupTransactionBuilder();

    private readonly IBackupTransactionExecutor _transactionExecutor = new BackupTransactionExecutor();
    public List<Job> Jobs { get; } = [];

    public JobManager LoadJobsFromFile(string filePath)
    {
        try
        {
            Jobs.AddRange(JobFileLoader.LoadJobsFromFile(filePath));
        }
        catch (Exception e)
        {
            throw new ArgumentException("errorInvalidJobFile", e);
        }

        return this;
    }

    public BackupCommand GetBackupCommandForIndexes(HashSet<int> jobIndexes)
    {
        List<Job> concernedJobs = new(jobIndexes.Select(i => Jobs[i-1]));

        return BuildBackupCommand(concernedJobs);
    }

    public BackupCommand BuildBackupCommand(List<Job> concernedJobs)
    {
        BackupTransaction transaction = _transactionBuilder.Build(concernedJobs);

        return new BackupCommand(_transactionExecutor, transaction);
    }

    public BackupCommand BuildBackupCommand(Job concernedJobs)
    {
        BackupTransaction transaction = _transactionBuilder.Build(concernedJobs);

        return new BackupCommand(_transactionExecutor, transaction);
    }
}
