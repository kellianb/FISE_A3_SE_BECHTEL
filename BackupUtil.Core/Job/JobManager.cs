using BackupUtil.Core.Command;
using BackupUtil.Core.Executor;
using BackupUtil.Core.Job.Exporter;
using BackupUtil.Core.Job.Loader;
using BackupUtil.Core.Transaction;
using BackupUtil.Core.Util;

namespace BackupUtil.Core.Job;

public class JobManager
{
    private readonly IBackupTransactionBuilder _transactionBuilder;

    internal JobManager(
        IBackupTransactionExecutor transactionExecutor,
        IBackupTransactionBuilder transactionBuilder,
        uint? maxJobs = null)
    {
        _transactionBuilder = transactionBuilder;
        MaxJobs = maxJobs is > 0 ? maxJobs.Value : Config.DefaultMaxJobCount;
    }


    public JobManager()
    {
        _transactionBuilder = new BackupTransactionBuilder();

        MaxJobs = Config.DefaultMaxJobCount;
    }

    public static string DefaultJobFilePath => Path.GetFullPath(Config.DefaultJobFilePath);

    public uint MaxJobs { get; }

    public List<Job> Jobs { get; } = [];

    #region Add jobs

    public JobManager AddJob(Job job)
    {
        if (Jobs.Count + 1 > MaxJobs)
        {
            throw new ArgumentException("errorMaxJobs");
        }

        Jobs.Add(job);
        return this;
    }

    public JobManager AddJobs(List<Job> jobs)
    {
        if (jobs.Count + Jobs.Count > MaxJobs)
        {
            throw new ArgumentException("errorMaxJobs");
        }

        Jobs.AddRange(jobs);
        return this;
    }

    public JobManager AddJobsFromFile(string? filePath = null)
    {
        List<Job> jobs;

        try
        {
            jobs = JobFileLoader.LoadJobsFromFile(filePath);
        }
        catch (Exception e)
        {
            throw new ArgumentException("errorInvalidJobFile", e);
        }

        AddJobs(jobs);

        return this;
    }

    #endregion

    #region Remove jobs

    public JobManager RemoveJobByIndex(int jobIndex)
    {
        Jobs.RemoveAt(jobIndex);
        return this;
    }

    public JobManager RemoveByIndices(HashSet<int> jobIndices)
    {
        foreach (int jobIndex in jobIndices.OrderDescending())
        {
            Jobs.RemoveAt(jobIndex);
        }

        return this;
    }

    public JobManager RemoveAll()
    {
        Jobs.Clear();
        return this;
    }

    #endregion

    #region Run jobs

    public BackupCommand RunByIndices(HashSet<int> jobIndices)
    {
        List<Job> concernedJobs = new(jobIndices.Select(i => Jobs[i]));

        return BuildBackupCommand(concernedJobs);
    }

    public BackupCommand RunAll()
    {
        return BuildBackupCommand(Jobs);
    }

    private BackupCommand BuildBackupCommand(List<Job> concernedJobs)
    {
        BackupTransaction transaction = _transactionBuilder.Build(concernedJobs);

        List<string> concernedJobNames = concernedJobs.Select(j => j.Name).ToList();

        return new BackupCommand(transaction, concernedJobNames);
    }

    private BackupCommand BuildBackupCommand(Job concernedJob)
    {
        BackupTransaction transaction = _transactionBuilder.Build(concernedJob);

        return new BackupCommand(transaction, [concernedJob.Name]);
    }

    #endregion

    #region Export jobs

    public JobManager ExportAll(string? filePath = null)
    {
        JobFileExporter.ExportJobsToFile(Jobs, filePath);
        return this;
    }

    public JobManager ExportByIndices(HashSet<int> jobIndices, string? filePath = null)
    {
        List<Job> concernedJobs = new(jobIndices.Select(i => Jobs[i]));
        JobFileExporter.ExportJobsToFile(concernedJobs, filePath);
        return this;
    }

    #endregion
}
