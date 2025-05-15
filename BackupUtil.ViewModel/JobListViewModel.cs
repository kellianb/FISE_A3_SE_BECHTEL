using System.Collections.ObjectModel;
using BackupUtil.Core.Job;

namespace BackupUtil.ViewModel;

public class JobListViewModel : ViewModelBase
{
    private readonly ObservableCollection<JobViewModel> _jobs;
    public LanguageSelectorViewModel LanguageSelectorViewModel { get; set; }


    public IEnumerable<JobViewModel> Jobs => _jobs;

    // public ICommand AddJobCommand { get; }

    public JobListViewModel()
    {
        _jobs = new ObservableCollection<JobViewModel>();

        _jobs.Add(new JobViewModel(new Job("a", "b",  true, true, "Hello world")));
        _jobs.Add(new JobViewModel(new Job()));
        _jobs.Add(new JobViewModel(new Job()));
        _jobs.Add(new JobViewModel(new Job()));

        LanguageSelectorViewModel = new LanguageSelectorViewModel();
    }
}
