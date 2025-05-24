using System.Windows;
using System.Windows.Controls;
using BackupUtil.I18n;
using BackupUtil.ViewModel.ViewModel;
using Microsoft.Win32;

namespace BackupUtil.Ui.View;

public partial class JobListingView : UserControl
{
    public JobListingView()
    {
        InitializeComponent();
        if (DataContext is JobListingViewModel viewModel)
        {
            viewModel.LanguageSelectorViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(LanguageSelectorViewModel.SelectedLanguage))
                {
                    viewModel.NotifyLocalizedMessagesChanged();
                }
            };
        }
    }

    public void SelectJobsPath(object sender, RoutedEventArgs routedEventArgs)
    {
        // Configure open file dialog box
        OpenFileDialog dialog = new()
        {
            Filter = "JSON Files (*.json)|*.json", // Filter files by extension
            Title = I18N.GetLocalizedMessage("selectJobsDialogTitle") // Set the title of the dialog
        };

        // Show open file dialog box
        bool? result = dialog.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            string filename = dialog.FileName;
            if (DataContext is JobListingViewModel viewModel)
            {
                viewModel.ChangeJobsPath(filename);
            }
        }
    }

    public void CreateJob(object sender, RoutedEventArgs routedEventArgs)
    {
        // TODO Fix
        // CreateJobView view = new(DataContext as JobListingViewModel);
        // view.Owner = Window.GetWindow(this);
        // view.ShowDialog();
    }
}
