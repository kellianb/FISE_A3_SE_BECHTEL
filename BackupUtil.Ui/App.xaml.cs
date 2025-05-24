using System.Windows;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.Ui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow { DataContext = new MainViewModel(NavigationStore.Instance) };

        MainWindow.Show();

        base.OnStartup(e);
    }
}
