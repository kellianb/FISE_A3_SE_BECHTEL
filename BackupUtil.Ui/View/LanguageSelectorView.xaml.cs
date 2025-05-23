using System.Windows;
using System.Windows.Controls;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.Ui.View;

public partial class LanguageSelectorView : UserControl
{
    public LanguageSelectorView()
    {
        InitializeComponent();
    }

    public void SelectLanguage(object sender, RoutedEventArgs routedEventArgs)
    {
        Console.WriteLine("selection " + languageComboBox.SelectedItem);
        if (DataContext is LanguageSelectorViewModel viewModel)
        {
            viewModel.ChangeLanguage(languageComboBox.SelectedItem.ToString());
        }
    }
}
