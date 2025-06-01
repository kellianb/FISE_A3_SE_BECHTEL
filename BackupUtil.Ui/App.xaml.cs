using System.Windows;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.Service;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace BackupUtil.Ui;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private Mutex? _mutex;

    public App()
    {
        IServiceCollection services = new ServiceCollection();

        // Shared objects
        services.AddSingleton(CreateJobStore);
        services.AddSingleton(CreateBackupCommandStore);
        services.AddSingleton<NavigationStore>();
        services.AddSingleton<ProgramFilterStore>();

        // ViewModels
        services.AddTransient(CreateMainViewModel);
        services.AddTransient(CreateHomeViewModel);
        services.AddTransient(CreateJobCreationViewModel);
        services.AddTransient(CreateJobListingViewModel);
        services.AddTransient(CreateSettingsViewModel);

        _serviceProvider = services.BuildServiceProvider();
    }


    #region App lifetime functions

    protected override void OnStartup(StartupEventArgs e)
    {
        if (!SingleInstance())
        {
            MessageBox.Show("Another instance of the application is already running.", Config.AppName);
            Environment.Exit(0);
        }

        _serviceProvider.GetRequiredService<NavigationStore>().CurrentViewModel =
            _serviceProvider.GetRequiredService<HomeViewModel>();

        MainWindow = new MainWindow { DataContext = _serviceProvider.GetRequiredService<MainViewModel>() };

        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _mutex?.ReleaseMutex();
        base.OnExit(e);
    }

    private bool SingleInstance()
    {
        _mutex = new Mutex(true, Config.AppName + "SingleInstance", out bool createdNew);

        return createdNew;
    }

    #endregion

    #region Create ViewModels

    private MainViewModel CreateMainViewModel(IServiceProvider serviceProvider)
    {
        return new MainViewModel(serviceProvider.GetRequiredService<NavigationStore>());
    }

    private JobListingViewModel CreateJobListingViewModel(IServiceProvider serviceProvider)
    {
        return new JobListingViewModel(serviceProvider.GetRequiredService<JobStore>());
    }

    private JobCreationViewModel CreateJobCreationViewModel(IServiceProvider serviceProvider)
    {
        return new JobCreationViewModel(serviceProvider.GetRequiredService<JobStore>(),
            CreateNavigationService<HomeViewModel>(serviceProvider));
    }

    private SettingsViewModel CreateSettingsViewModel(IServiceProvider serviceProvider)
    {
        return new SettingsViewModel(serviceProvider.GetRequiredService<ProgramFilterStore>(),
            CreateNavigationService<HomeViewModel>(serviceProvider));
    }

    private HomeViewModel CreateHomeViewModel(IServiceProvider serviceProvider)
    {
        return new HomeViewModel(CreateJobListingViewModel(serviceProvider),
            CreateTransactionListingViewModel(serviceProvider),
            CreateNavigationService<JobCreationViewModel>(serviceProvider),
            CreateNavigationService<SettingsViewModel>(serviceProvider));
    }

    private TransactionListingViewModel CreateTransactionListingViewModel(IServiceProvider serviceProvider)
    {
        return new TransactionListingViewModel(serviceProvider.GetRequiredService<BackupCommandStore>());
    }

    #endregion

    #region Create stores

    private NavigationService<TViewModel> CreateNavigationService<TViewModel>(IServiceProvider serviceProvider)
        where TViewModel : ViewModelBase
    {
        return new NavigationService<TViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
            serviceProvider.GetRequiredService<TViewModel>);
    }

    private JobStore CreateJobStore(IServiceProvider serviceProvider)
    {
        return new JobStore(serviceProvider.GetRequiredService<BackupCommandStore>());
    }

    private BackupCommandStore CreateBackupCommandStore(IServiceProvider serviceProvider)
    {
        return new BackupCommandStore(serviceProvider.GetRequiredService<ProgramFilterStore>());
    }

    #endregion
}
