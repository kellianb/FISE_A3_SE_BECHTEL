using System.Collections.ObjectModel;
using System.Windows.Input;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class JobListingViewModel : ViewModelBase
{
    private readonly JobManager _jobManager;
    private readonly ObservableCollection<JobViewModel> _jobs;

    public JobListingViewModel(JobManager jobManager, NavigationStore navigationStore)
    {
        CreateJobCommand = new NavigateCommand(navigationStore);

        _jobManager = jobManager.AddJobsFromFile();
        _jobs = [];

        foreach (Job job in _jobManager.Jobs)
        {
            _jobs.Add(new JobViewModel(job));
        }
    }

    public IEnumerable<JobViewModel> Jobs => _jobs;

    public ICommand CreateJobCommand { get; }
    public ICommand LoadJobsCommand { get; }
}
