using System.ComponentModel;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateJobCommand : CommandBase
{
    private readonly JobCreationViewModel _jobCreationViewModel;
    private readonly JobManager _jobManager;

    public CreateJobCommand(JobCreationViewModel jobCreationViewModel, JobManager jobManager)
    {
        _jobManager = jobManager;
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
            _jobCreationViewModel.EncryptionType,
            _jobCreationViewModel.EncryptionKey,
            _jobCreationViewModel.FileMask);

        _jobManager.AddJob(job);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_jobCreationViewModel.CanCreateJob))
        {
            OnCanExecuteChanged();
        }
    }
}
