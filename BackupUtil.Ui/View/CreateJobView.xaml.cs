using System.IO;
using System.Windows;
using System.Windows.Controls;
using BackupUtil.Crypto;
using BackupUtil.I18n;
using BackupUtil.ViewModel.ViewModel;
using Microsoft.Win32;

namespace BackupUtil.Ui.View;

public partial class CreateJobView : Window
{
    private FileSystemInfo _sourcePath;
    private FileSystemInfo _targetPath;
    private readonly JobListingViewModel _jobListingViewModel;
    private TextBlock _encryptionKeyLabel;
    private TextBox _encryptionKeyInput;

    public CreateJobView(JobListingViewModel jobListingViewModel)
    {
        InitializeComponent();
        _jobListingViewModel = jobListingViewModel;
        _encryptionKeyLabel = new()
        {
            Name = "EncryptionKeyLabel",
            Text = I18N.GetLocalizedMessage("encryptionKeyLabel"),
            Margin= new Thickness(0, 0, 0, 5)
        };
        _encryptionKeyInput = new()
        {
            Name = "EncryptionKeyInput",
            HorizontalAlignment= HorizontalAlignment.Stretch
        };
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
        _jobListingViewModel.AddJob(
            _sourcePath,
            _targetPath,
            RecursiveInput.IsChecked ?? false,
            DifferentialInput.IsChecked ?? false,
            NameInput.Text
        );

        Close();
    }

    public void SelectEncryptionType(object sender, RoutedEventArgs routedEventArgs)
    {
            string? selectedType = EncryptionTypeInput.SelectedItem.ToString();
            if (selectedType != null && selectedType != I18N.GetLocalizedMessage("noEncryption") && !EncryptionPanel.Children.Contains(_encryptionKeyInput))
            {
                EncryptionPanel.Children.Add(_encryptionKeyLabel);
                EncryptionPanel.Children.Add(_encryptionKeyInput);
            }
            else if (selectedType == I18N.GetLocalizedMessage("noEncryption") && EncryptionPanel.Children.Contains(_encryptionKeyInput))
            {
                EncryptionPanel.Children.Remove(_encryptionKeyInput);
                EncryptionPanel.Children.Remove(_encryptionKeyLabel);
            }
    }
}
