using MahApps.Metro.Controls; // ✅ Import MahApps.Metro
using BackupUtil.Core.Util;

namespace BackupUtil.Ui;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MetroWindow // ✅ Change Window -> MetroWindow
{
    public MainWindow()
    {
        Title = Config.AppName;

        InitializeComponent();
    }
}
