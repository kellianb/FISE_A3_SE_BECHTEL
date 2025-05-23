using System.ComponentModel;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command;

public class CreateJobCommand : AsyncCommandBase
{
    private readonly JobManager _jobManager;
    private readonly JobCreationViewModel _jobCreationViewModel;

    public CreateJobCommand(JobManager jobManager, JobCreationViewModel jobCreationViewModel)
    {
        _jobManager = jobManager;
        _jobCreationViewModel = jobCreationViewModel;
        _jobCreationViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override void Execute(object? parameter)
    {
    }

    public override Task ExecuteAsync(object? parameter)
    {
        Job job = new("", "", false, false);

        try
        {
            _jobManager.AddJob(job);
        }
        catch {}
        
        return Task.CompletedTask;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(_jobCreationViewModel.CanCreateJob))
        {
            OnCanExecuteChanged();
        }
    }
}
