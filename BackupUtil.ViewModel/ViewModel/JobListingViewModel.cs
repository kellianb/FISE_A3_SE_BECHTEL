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
    private readonly BackupCommandStore _backupCommandStore;
    private readonly JobStore _jobStore;

    public JobListingViewModel(JobStore jobStore, BackupCommandStore backupCommandStore)
    {
        _jobStore = jobStore;
        _backupCommandStore = backupCommandStore;

        _jobStore.PropertyChanged += OnJobStorePropertyChanged;

        LoadJobsCommand = new LoadJobsCommand(this, _jobStore);
        ExportJobsCommand = new ExportJobsCommand(this, _jobStore);
        DeleteSelectedJobsCommand = new DeleteSelectedJobsCommand(this, _jobStore);
        CreateTransactionsForSelectedJobsCommand =
            new CreateTransactionsForSelectedJobsCommand(this, _jobStore, _backupCommandStore);

        LoadJobViewModels();
    }


    public bool CanAccessJobFile => _jobStore.CanAccessJobFile;

    #region JobFilePath

    public string JobFilePath
    {
        get => _jobStore.JobFilePath;
        set => _jobStore.JobFilePath = value;
    }

    #endregion

    // Unsubscribe from eventHandlers when disposing this viewModel
    public override void Dispose()
    {
        DisposeJobViewModels();
        _jobStore.PropertyChanged -= OnJobStorePropertyChanged;
        base.Dispose();
    }

    #region Handle JobStore events

    // Transmit events emitted in the JobStore to the view
    private void OnJobStorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(JobStore.Jobs):
                LoadJobViewModels(); // LoadJobs calls OnPropertyChanged
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

    #region Handle JobViewModel events

    private void OnJobViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(JobViewModel.IsSelected))
        {
            OnPropertyChanged(nameof(SelectedJobIndices));
        }
    }

    #endregion

    #region Jobs

    public ObservableCollection<JobViewModel> Jobs { get; } = [];

    // Determine the indices of selected JobViewModels
    public HashSet<int> SelectedJobIndices
    {
        get
        {
            HashSet<int> indices = [];
            for (int i = 0; i < Jobs.Count; i++)
            {
                if (Jobs[i].IsSelected)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }
    }

    /// <summary>
    ///     Create and Load JobViewModels from the JobStore
    /// </summary>
    public void LoadJobViewModels()
    {
        DisposeJobViewModels();

        foreach (Job job in _jobStore.Jobs)
        {
            JobViewModel jobViewModel = new(job);
            jobViewModel.PropertyChanged += OnJobViewModelPropertyChanged;
            Jobs.Add(jobViewModel);
        }

        OnPropertyChanged(nameof(SelectedJobIndices));
    }

    /// <summary>
    ///     Get rid of all JobViewModels
    /// </summary>
    private void DisposeJobViewModels()
    {
        foreach (JobViewModel jobViewModel in Jobs)
        {
            jobViewModel.PropertyChanged -= OnJobViewModelPropertyChanged;
            jobViewModel.Dispose();
        }

        Jobs.Clear();
    }

    #endregion

    #region Commands

    // Loads jobs from the job file
    public ICommand LoadJobsCommand { get; }

    // Save jobs to the job file
    public ICommand ExportJobsCommand { get; }

    // Create a transaction from all jobs for which IsSelected is true
    public ICommand CreateTransactionsForSelectedJobsCommand { get; }

    // Delete all jobs for which IsSelected is true
    public ICommand DeleteSelectedJobsCommand { get; }

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
