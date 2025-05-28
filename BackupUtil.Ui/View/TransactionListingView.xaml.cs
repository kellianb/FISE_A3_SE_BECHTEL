using System.Windows.Controls;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.Ui.View;

public partial class TransactionListingView : UserControl
{
    public TransactionListingView()
    {
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (object addedItem in e.AddedItems)
        {
            if (addedItem is TransactionViewModel transactionViewModel)
            {
                transactionViewModel.IsSelected = true;
            }
        }

        foreach (object removedItem in e.RemovedItems)
        {
            if (removedItem is TransactionViewModel transactionViewModel)
            {
                transactionViewModel.IsSelected = false;
            }
        }
    }
}
