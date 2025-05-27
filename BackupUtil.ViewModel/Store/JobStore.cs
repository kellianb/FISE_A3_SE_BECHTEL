using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackupUtil.Core.Job;

namespace BackupUtil.ViewModel.Store;

public class JobStore : INotifyPropertyChanged
{
    private readonly JobManager _jobManager;

    public JobStore()
    {
        _jobManager = new JobManager();
        _jobFilePath = JobManager.DefaultJobFilePath;
    }

    public bool CanAccessJobFile => JobFileExists;


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Jobs

    public List<Job> Jobs => _jobManager.Jobs;

    // Add a single job
    public void AddJob(Job job)
    {
        _jobManager.AddJob(job);
        OnPropertyChanged(nameof(Jobs));
    }

    public void RemoveJobsByIndex(HashSet<int> indices)
    {
        _jobManager.RemoveByIndices(indices);
        OnPropertyChanged(nameof(Jobs));
    }

    public void RemoveAll()
    {
        _jobManager.RemoveAll();
        OnPropertyChanged(nameof(Jobs));
    }

    #endregion

    #region JobFilePath

    private string _jobFilePath;

    private bool JobFileExists => File.Exists(_jobFilePath);

    public string JobFilePath
    {
        get => _jobFilePath;
        set
        {
            _jobFilePath = value;
            OnPropertyChanged();

            // Determine errors
            ClearErrors();

            if (!JobFileExists)
            {
                AddError("errorInvalidJobFile");
            }

            OnPropertyChanged(nameof(CanAccessJobFile));
        }
    }

    #endregion

    #region Load and export

    public void LoadJobs()
    {
        _jobManager.AddJobsFromFile(JobFilePath);
        OnPropertyChanged(nameof(Jobs));
    }

    public void ExportAll()
    {
        _jobManager.ExportAll(JobFilePath);
        OnPropertyChanged(nameof(CanAccessJobFile));
    }

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
