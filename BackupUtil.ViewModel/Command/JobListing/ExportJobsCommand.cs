using System.ComponentModel;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.JobListing;

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
        try
        {
            _jobManager.ExportAll();
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.CanAccessJobFile))
        {
            OnCanExecuteChanged();
        }
    }
}
