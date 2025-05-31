using System.Windows;
using BackupUtil.Core.Util;

namespace BackupUtil.Ui;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Title = Config.AppName;

        InitializeComponent();
    }
}
