using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BackupUtil.Core.Job;
using BackupUtil.ViewModel.Command;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class JobListingViewModel : ViewModelBase
{
    private readonly JobStore _jobStore;
    private ObservableCollection<JobViewModel> _jobs = [];

    public JobListingViewModel(JobStore jobStore)
    {
        _jobStore = jobStore;

        _jobStore.PropertyChanged += OnJobStorePropertyChanged;

        LoadJobsCommand = new LoadJobsCommand(this, _jobStore);
        ExportJobsCommand = new ExportJobsCommand(this, _jobStore);

        LoadJobs();
    }

    public IEnumerable<JobViewModel> Jobs => _jobs;

    public bool CanAccessJobFile => _jobStore.CanAccessJobFile;

    #region JobFilePath

    public string JobFilePath
    {
        get => _jobStore.JobFilePath;
        set => _jobStore.JobFilePath = value;
    }

    #endregion

    public void LoadJobs()
    {
        _jobs = [];

        foreach (Job job in _jobStore.Jobs)
        {
            _jobs.Add(new JobViewModel(job));
        }

        OnPropertyChanged(nameof(Jobs));
    }

    #region Handle JobStore events

    private void OnJobStorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(JobStore.Jobs):
                LoadJobs(); // LoadJobs calls OnPropertyChanged
                break;
            case nameof(JobStore.JobFilePath):
                OnPropertyChanged(nameof(JobFilePath));
                break;
            case nameof(JobStore.CanAccessJobFile):
                OnPropertyChanged(nameof(CanAccessJobFile));
                break;
        }
    }

    #endregion

    #region Commands

    // Loads jobs from the job file
    public ICommand LoadJobsCommand { get; }

    // Save jobs to the job file
    public ICommand ExportJobsCommand { get; }

    #endregion

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

    public bool HasErrors => _propertyNameToErrorsDictionary.Count != 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    private void AddError(string errorMessage, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        if (!_propertyNameToErrorsDictionary.TryGetValue(propertyName, out List<string>? value))
        {
            value = [];
            _propertyNameToErrorsDictionary.Add(propertyName, value);
        }

        value.Add(errorMessage);

        OnErrorsChanged(propertyName);
    }

    private void ClearErrors([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        _propertyNameToErrorsDictionary.Remove(propertyName);

        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged([CallerMemberName] string? propertyName = null)
    {
        if (propertyName is null)
        {
            return;
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    #endregion
}
