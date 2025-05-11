using BackupUtil.Core.Job.Loader;

namespace BackupUtil.Core.Job;

public class JobManager
{
    public List<Job> Jobs { get; } = [];

    public void LoadJobsFromFile(string filePath)
    {
        Jobs.AddRange(JobFileLoader.LoadJobsFromFile(filePath));
    }
}
