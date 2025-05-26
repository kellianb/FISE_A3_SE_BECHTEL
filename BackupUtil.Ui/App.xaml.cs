using System.Globalization;
using System.Windows;
using BackupUtil.Core.Job;
using BackupUtil.I18n;
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
        services.AddSingleton<JobManager>();
        services.AddSingleton<NavigationStore>();

        // ViewModels
        services.AddTransient(CreateMainViewModel);
        services.AddTransient(CreateJobCreationViewModel);
        services.AddTransient(CreateJobListingViewModel);

        _serviceProvider = services.BuildServiceProvider();
    }


    protected override void OnStartup(StartupEventArgs e)
    {
        I18N.SetCulture(new CultureInfo("fr-fr"));
        _serviceProvider.GetRequiredService<NavigationStore>().CurrentViewModel =
            _serviceProvider.GetRequiredService<JobListingViewModel>();

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
        return new JobListingViewModel(serviceProvider.GetRequiredService<JobManager>(),
            CreateNavigationService<JobCreationViewModel>(serviceProvider));
    }

    private JobCreationViewModel CreateJobCreationViewModel(IServiceProvider serviceProvider)
    {
        return new JobCreationViewModel(serviceProvider.GetRequiredService<JobManager>(),
            CreateNavigationService<JobListingViewModel>(serviceProvider));
    }

    private NavigationService<TViewModel> CreateNavigationService<TViewModel>(IServiceProvider serviceProvider)
        where TViewModel : ViewModelBase
    {
        return new NavigationService<TViewModel>(serviceProvider.GetRequiredService<NavigationStore>(),
            serviceProvider.GetRequiredService<TViewModel>);
    }
}
