using System.Windows.Controls;
using BackupUtil.I18n;
using BackupUtil.ViewModel;

namespace BackupUtil.Ui.View;

public partial class LanguageSelectorView : UserControl
{
    public LanguageSelectorView()
    {
        InitializeComponent();
        DataContext = new LanguageSelectorViewModel();
    }
}

