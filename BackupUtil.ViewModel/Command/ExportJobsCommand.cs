using System.ComponentModel;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class ExportJobsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobManager;

    public ExportJobsCommand(JobListingViewModel jobListingViewModel, JobStore jobManager)
    {
        _jobManager = jobManager;
        _jobListingViewModel = jobListingViewModel;
        _jobListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override void Execute(object? parameter)
    {
        _jobManager.ExportAll();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.CanAccessJobFile))
        {
            OnCanExecuteChanged();
        }
    }
}
