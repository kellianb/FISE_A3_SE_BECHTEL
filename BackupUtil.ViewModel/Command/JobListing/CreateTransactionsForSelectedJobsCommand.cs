using System.ComponentModel;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.JobListing;

public class CreateTransactionsForSelectedJobsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;

    public CreateTransactionsForSelectedJobsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore)
    {
        _jobStore = jobStore;
        _jobListingViewModel = jobListingViewModel;
        _jobListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.SelectedJobIndices.Count > 0 && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        try
        {
            _jobStore.RunByIndices(_jobListingViewModel.SelectedJobIndices);
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.SelectedJobIndices))
        {
            OnCanExecuteChanged();
        }
    }
}
