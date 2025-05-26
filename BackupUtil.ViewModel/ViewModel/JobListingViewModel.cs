using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Service;

namespace BackupUtil.ViewModel.ViewModel;

public class JobListingViewModel : ViewModelBase
{
    private readonly JobManager _jobManager;
    private ObservableCollection<JobViewModel> _jobs = [];

    public JobListingViewModel(JobManager jobManager, NavigationService<JobCreationViewModel> navigationService)
    {
        _jobManager = jobManager;

        CreateJobCommand = new NavigateCommand<JobCreationViewModel>(navigationService);
        LoadJobsCommand = new LoadJobsCommand(this, _jobManager);
        ExportJobsCommand = new ExportJobsCommand(this, _jobManager);

        LoadJobs();
    }

    public IEnumerable<JobViewModel> Jobs => _jobs;

    public ICommand CreateJobCommand { get; }
    public ICommand LoadJobsCommand { get; }

    public ICommand ExportJobsCommand { get; }

    public LanguageSelectionViewModel LanguageSelectionViewModel { get; } = new();

    public bool CanAccessJobFile => JobFileExists;

    public void LoadJobs()
    {
        _jobs = [];

        foreach (Job job in _jobManager.Jobs)
        {
            _jobs.Add(new JobViewModel(job));
        }

        OnPropertyChanged(nameof(Jobs));
    }

    #region Error handling

    private readonly Dictionary<string, List<string>> _propertyNameToErrorsDictionary = new();

    public IEnumerable GetErrors(string? propertyName)
    {
        if (propertyName == null ||
            !_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? errors))
        {
            return new List<string>();
        }

        return errors;
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    private void AddError(string errorMessage, string propertyName)
    {
        if (!_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            _propertyNameToErrorsDictionary.Add(propertyName, value);
        }

        value.Add(errorMessage);

        OnErrorsChanged(propertyName);
    }

    private void ClearErrors(string propertyName)
    {
        _propertyNameToErrorsDictionary.Remove(propertyName);

        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion

    #region JobFilePath

    private string _jobFilePath = JobManager.DefaultJobFilePath;

    private bool JobFileExists => File.Exists(_jobFilePath);

    public string JobFilePath
    {
        get => _jobFilePath;
        set
        {
            _jobFilePath = value;
            OnPropertyChanged();

            // Determine errors
            ClearErrors(nameof(JobFilePath));

            if (!JobFileExists)
            {
                AddError("errorInvalidJobFile", nameof(JobFilePath));
            }

            OnPropertyChanged(nameof(CanAccessJobFile));
        }
    }

    #endregion
}
