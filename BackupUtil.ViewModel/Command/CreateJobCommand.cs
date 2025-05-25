using System.ComponentModel;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateJobCommand<TViewModel> : CommandBase where TViewModel : ViewModelBase
{
    private readonly JobCreationViewModel _jobCreationViewModel;
    private readonly JobManager _jobManager;
    private readonly NavigationService<TViewModel> _navigationService;

    public CreateJobCommand(JobCreationViewModel jobCreationViewModel, JobManager jobManager,
        NavigationService<TViewModel> navigationService)
    {
        _jobManager = jobManager;
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
        Job job = new(_jobCreationViewModel.SourcePath,
            _jobCreationViewModel.TargetPath,
            _jobCreationViewModel.Recursive,
            _jobCreationViewModel.Differential,
            _jobCreationViewModel.Name,
            EncryptionTypeOptionsUtils.To(_jobCreationViewModel.EncryptionType),
            _jobCreationViewModel.EncryptionKey,
            _jobCreationViewModel.FileMask);

        _jobManager.AddJob(job);
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
