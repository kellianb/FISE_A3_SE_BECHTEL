using System.ComponentModel;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class DeleteSelectedJobsCommand : CommandBase
{
    private readonly JobListingViewModel _jobListingViewModel;
    private readonly JobStore _jobStore;

    public DeleteSelectedJobsCommand(JobListingViewModel jobListingViewModel, JobStore jobStore)
    {
        _jobStore = jobStore;
        _jobListingViewModel = jobListingViewModel;
        _jobListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override void Execute(object? parameter)
    {
        _jobStore.RemoveByIndices(_jobListingViewModel.SelectedJobIndices);
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobListingViewModel.SelectedJobIndices.Count > 0 && base.CanExecute(parameter);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobListingViewModel.SelectedJobIndices))
        {
            OnCanExecuteChanged();
        }
    }
}
