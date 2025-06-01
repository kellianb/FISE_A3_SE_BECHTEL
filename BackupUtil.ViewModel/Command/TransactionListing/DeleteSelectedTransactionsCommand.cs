using System.ComponentModel;
using BackupUtil.Core.Util;
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
        try
        {
            _backupCommandStore.RemoveByIndices(_transactionListingViewModel.SelectedTransactionIndices);
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }
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
