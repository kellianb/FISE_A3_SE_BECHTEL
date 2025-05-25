using System.Globalization;
using System.Windows;
using BackupUtil.Core.Job;
using BackupUtil.I18n;
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
        _navigationStore.CurrentViewModel = new JobListingViewModel(_jobManager, _navigationStore);
        // _navigationStore.CurrentViewModel = new JobCreationViewModel(_jobManager);

        MainWindow = new MainWindow { DataContext = new MainViewModel(_navigationStore) };

        MainWindow.Show();

        base.OnStartup(e);
    }
}
