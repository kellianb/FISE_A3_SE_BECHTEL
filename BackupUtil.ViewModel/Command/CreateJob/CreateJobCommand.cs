using System.ComponentModel;
using BackupUtil.Core.Job;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.CreateJob;

public class CreateJobCommand<TViewModel> : CommandBase where TViewModel : ViewModelBase
{
    private readonly JobCreationViewModel _jobCreationViewModel;
    private readonly JobStore _jobStore;
    private readonly NavigationService<TViewModel> _navigationService;

    public CreateJobCommand(JobCreationViewModel jobCreationViewModel, JobStore jobStore,
        NavigationService<TViewModel> navigationService)
    {
        _jobStore = jobStore;
        _navigationService = navigationService;
        _jobCreationViewModel = jobCreationViewModel;
        _jobCreationViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override bool CanExecute(object? parameter)
    {
        return _jobCreationViewModel.CanCreateJob && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        try
        {
            Job job = new(_jobCreationViewModel.SourcePath,
                _jobCreationViewModel.TargetPath,
                _jobCreationViewModel.Recursive,
                _jobCreationViewModel.Differential,
                _jobCreationViewModel.Name,
                EncryptionTypeOptionsUtils.To(_jobCreationViewModel.EncryptionType),
                _jobCreationViewModel.EncryptionKey,
                _jobCreationViewModel.FileMask);

            _jobStore.AddJob(job);
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }

        _navigationService.Navigate();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobCreationViewModel.CanCreateJob))
        {
            OnCanExecuteChanged();
        }
    }
}
