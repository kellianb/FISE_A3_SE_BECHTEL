using System.Globalization;
using System.Windows;
using BackupUtil.Core.Job;
using BackupUtil.I18n;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.Ui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly JobManager _jobManager = new();
    private readonly NavigationStore _navigationStore = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        I18N.SetCulture(new CultureInfo("fr-fr"));
        _navigationStore.CurrentViewModel = CreateJobListingViewModel();

        MainWindow = new MainWindow { DataContext = new MainViewModel(_navigationStore) };

        MainWindow.Show();

        base.OnStartup(e);
    }

    private JobListingViewModel CreateJobListingViewModel()
    {
        return new JobListingViewModel(_jobManager,
            new NavigationService(_navigationStore, CreateJobCreationViewModel));
    }

    private JobCreationViewModel CreateJobCreationViewModel()
    {
        return new JobCreationViewModel(_jobManager,
            new NavigationService(_navigationStore, CreateJobListingViewModel));
    }
}
