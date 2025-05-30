using System.ComponentModel;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

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
        _jobStore.RunByIndices(_jobListingViewModel.SelectedJobIndices);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.SelectedJobIndices))
        {
            OnCanExecuteChanged();
        }
    }
}
