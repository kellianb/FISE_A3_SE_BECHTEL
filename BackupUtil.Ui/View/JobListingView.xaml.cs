using System.Windows.Controls;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.Ui.View;

public partial class JobListingView : UserControl
{
    public JobListingView()
    {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (object addedItem in e.AddedItems)
        {
            if (addedItem is JobViewModel job)
            {
                job.IsSelected = true;
            }
        }

        foreach (object removedItem in e.RemovedItems)
        {
            if (removedItem is JobViewModel job)
            {
                job.IsSelected = false;
            }
        }
    }
}
