using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using BackupUtil.ViewModel.Service;

namespace BackupUtil.Ui.View;

public partial class LanguageSelectorView : UserControl
{
    public LanguageSelectorView()
    {
        InitializeComponent();
    }

    public void SelectLanguage(object sender, RoutedEventArgs routedEventArgs)
    {
        string? lang = languageComboBox.SelectedItem.ToString();

        if (lang is null)
        {
            return;
        }

        LocalizationService.Instance.ChangeLanguage(new CultureInfo(lang));
    }
}
