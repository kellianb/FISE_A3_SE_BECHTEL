using BackupUtil.Core.Job;

namespace BackupUtil.ViewModel.Command;

public class CreateJobCommand(JobManager jobManager) : CommandBase
{
    private readonly JobManager _jobManager = jobManager;

    public override void Execute(object? parameter)
    {
    }
}
