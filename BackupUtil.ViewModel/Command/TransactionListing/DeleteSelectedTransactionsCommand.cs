using System.ComponentModel;
using BackupUtil.ViewModel.Store;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.TransactionListing;

public class DeleteSelectedTransactionsCommand : CommandBase
{
    private readonly BackupCommandStore _backupCommandStore;
    private readonly TransactionListingViewModel _transactionListingViewModel;

    public DeleteSelectedTransactionsCommand(TransactionListingViewModel transactionListingViewModel,
        BackupCommandStore backupCommandStore)
    {
        _backupCommandStore = backupCommandStore;
        _transactionListingViewModel = transactionListingViewModel;
        _transactionListingViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public override void Execute(object? parameter)
    {
        _backupCommandStore.RemoveByIndices(_transactionListingViewModel.SelectedTransactionIndices);
    }

    public override bool CanExecute(object? parameter)
    {
        return _transactionListingViewModel.SelectedTransactionIndices.Count > 0 && base.CanExecute(parameter);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_transactionListingViewModel.SelectedTransactionIndices))
        {
            OnCanExecuteChanged();
        }
    }
}
