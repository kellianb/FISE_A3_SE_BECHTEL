using System.IO;
using System.Windows;
using BackupUtil.I18n;
using BackupUtil.ViewModel;
using Microsoft.Win32;

namespace BackupUtil.Ui.View;

public partial class ConfigureJobWindow : Window
{
    private FileSystemInfo _sourcePath;
    private FileSystemInfo _targetPath;
    private readonly JobListViewModel jobListViewModel;

    public ConfigureJobWindow(JobListViewModel jobListViewModel)
    {
        InitializeComponent();
        this.jobListViewModel = jobListViewModel;
    }

    public void SelectSourcePath(object sender, RoutedEventArgs routedEventArgs)
    {
        OpenFolderDialog dialog = new();

        // Show the dialog
        if (dialog.ShowDialog() == true)
        {
            if (Directory.Exists(dialog.FolderName))
            {
                _sourcePath = new DirectoryInfo(dialog.FolderName);
            }
            else
            {
                Console.WriteLine(I18N.GetLocalizedMessage("invalidPath"));
            }
        }
    }

    public void SelectTargetPath(object sender, RoutedEventArgs routedEventArgs)
    {
        OpenFolderDialog dialog = new();

        // Show the dialog
        if (dialog.ShowDialog() == true)
        {
            if (Directory.Exists(dialog.FolderName))
            {
                _targetPath = new DirectoryInfo(dialog.FolderName);
            }
            else
            {
                Console.WriteLine(I18N.GetLocalizedMessage("invalidPath"));
            }
        }
    }

    public void SendJobData(object sender, RoutedEventArgs routedEventArgs)
    {
        //TODO: mandatory fields + add encryptor type (not mandatory) and encryptor key (mandatory if type is not null)
        jobListViewModel.AddJob(
            _sourcePath,
            _targetPath,
            RecursiveInput.IsChecked ?? false,
            DifferentialInput.IsChecked ?? false,
            NameInput.Text
        );

        Close();
    }
}
