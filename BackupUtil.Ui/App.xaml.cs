using System.Windows;
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

    public App()
    {
        IServiceCollection services = new ServiceCollection();

        // Shared objects
        services.AddSingleton<JobStore>();
        services.AddSingleton<BackupCommandStore>();
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


    protected override void OnStartup(StartupEventArgs e)
    {
        _serviceProvider.GetRequiredService<NavigationStore>().CurrentViewModel =
            _serviceProvider.GetRequiredService<HomeViewModel>();

        MainWindow = new MainWindow { DataContext = _serviceProvider.GetRequiredService<MainViewModel>() };

        MainWindow.Show();

        base.OnStartup(e);
    }

    private MainViewModel CreateMainViewModel(IServiceProvider serviceProvider)
    {
        return new MainViewModel(serviceProvider.GetRequiredService<NavigationStore>());
    }

    private JobListingViewModel CreateJobListingViewModel(IServiceProvider serviceProvider)
    {
        return new JobListingViewModel(serviceProvider.GetRequiredService<JobStore>(),
            serviceProvider.GetRequiredService<BackupCommandStore>());
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

    private NavigationService<TViewModel> CreateNavigationService<TViewModel>(IServiceProvider serviceProvider)
        where TViewModel : ViewModelBase
    {
        return new NavigationService<TViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
            serviceProvider.GetRequiredService<TViewModel>);
    }
}
