using System.Collections.ObjectModel;
using BackupUtil.Core.Job;
using BackupUtil.I18n;

namespace BackupUtil.ViewModel;

public class JobListViewModel : ViewModelBase
{
    // public ICommand AddJobCommand { get; }

    public JobListViewModel()
    {
        // Create the default directories if they doesn't exist
        Directory.CreateDirectory(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EasySave", "jobs"));
        jobFilePath = new FileInfo(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EasySave", "jobs", "BackupJobs.json"));
        LanguageSelectorViewModel = new LanguageSelectorViewModel();
        _jobs = new ObservableCollection<JobViewModel>();

        try
        {
            JobManager manager = new();

            try
            {
                manager.AddJobsFromFile(jobFilePath?.FullName);
            }
            catch
            {
                // Only catch the exception if the default job file path was used
                if (jobFilePath is not null)
                {
                    throw;
                }
            }

            // Add jobs to the collection
            foreach (Job job in manager.Jobs)
            {
                _jobs.Add(new JobViewModel(job));
            }
        }
        catch (Exception ex)
        {
            // Handle errors (e.g., invalid file format)
            Console.WriteLine(ex.Message);
        }
    }

    private static ObservableCollection<JobViewModel> _jobs { get; set; }
    public LanguageSelectorViewModel LanguageSelectorViewModel { get; }

    public FileInfo jobFilePath { get; set; }

    public Dictionary<string, string> LocalizedMessages => new()
    {
        { "jobName", I18N.GetLocalizedMessage("jobName") },
        { "jobTargetPath", I18N.GetLocalizedMessage("jobTargetPath") },
        { "jobSourcePath", I18N.GetLocalizedMessage("jobSourcePath") },
        { "jobRecursive", I18N.GetLocalizedMessage("jobRecursive") },
        { "jobDifferential", I18N.GetLocalizedMessage("jobDifferential") },
        { "addJob", I18N.GetLocalizedMessage("addJob") },
        { "changeJobFile", I18N.GetLocalizedMessage("changeJobFile") }
    };

    public IEnumerable<JobViewModel> Jobs => _jobs;

    public void ChangeJobsPath(string filename)
    {
        //TODO: linux version + implement in view
        try
        {
            jobFilePath = new FileInfo(filename);
            // Load jobs from the selected file
            JobManager manager = new();
            manager.AddJobsFromFile(filename);

            // Clear the current jobs and add the new ones
            _jobs.Clear();
            foreach (Job job in manager.Jobs)
            {
                _jobs.Add(new JobViewModel(job));
            }
        }
        catch (Exception ex)
        {
            // Handle errors (e.g., invalid file format)
            Console.WriteLine(ex.Message);
        }
    }

    //TODO: implement in view + add creation of file if not exist
    public void AddJob(FileSystemInfo sourcePath, FileSystemInfo targetPath, bool recursive, bool differential,
        string name = null)
    {
        if (!jobFilePath.Exists)
        {
            // Create the file and initialize it with empty JSON brackets
            using (StreamWriter writer = new(jobFilePath.FullName))
            {
                writer.Write("[]");
            }
        }

        Job job = new(sourcePath.FullName, targetPath.FullName, recursive, differential, name);

        try
        {
            JobManager manager = new();

            try
            {
                manager.AddJobsFromFile(jobFilePath?.FullName);
            }
            catch
            {
                // Only catch the exception if the default job file path was used
                if (jobFilePath is not null)
                {
                    throw;
                }
            }

            manager.AddJob(job)
                .ExportAll(jobFilePath?.FullName);
        }
        catch (Exception e)
        {
            Console.WriteLine(I18N.GetLocalizedMessage(e.Message));
        }
    }

    public void NotifyLocalizedMessagesChanged()
    {
        OnPropertyChanged(nameof(LocalizedMessages));
    }
}
