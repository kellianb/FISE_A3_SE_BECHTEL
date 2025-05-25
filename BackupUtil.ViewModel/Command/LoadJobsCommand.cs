using System.ComponentModel;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class LoadJobsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobManager _jobManager;

    public LoadJobsCommand(JobListingViewModel jobListingViewModel, JobManager jobManager)
    {
        _jobManager = jobManager;
        _jobListingViewModel = jobListingViewModel;
        _jobListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.CanAccessJobFile && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _jobManager.AddJobsFromFile(_jobListingViewModel.JobFilePath);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.CanAccessJobFile))
        {
            OnCanExecuteChanged();
        }
    }
}
