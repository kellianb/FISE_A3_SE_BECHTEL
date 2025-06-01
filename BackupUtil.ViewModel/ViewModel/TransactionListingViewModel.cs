using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using BackupUtil.Core.Command;
using BackupUtil.ViewModel.Command.TransactionListing;
using BackupUtil.ViewModel.Store;

namespace BackupUtil.ViewModel.ViewModel;

public class TransactionListingViewModel : ViewModelBase
{
    private readonly BackupCommandStore _backupCommandStore;

    public TransactionListingViewModel(BackupCommandStore backupCommandStore)
    {
        _backupCommandStore = backupCommandStore;

        _backupCommandStore.PropertyChanged += OnBackupCommandStorePropertyChanged;

        DeleteSelectedTransactionsCommand = new DeleteSelectedTransactionsCommand(this, _backupCommandStore);

        LoadTransactionViewModels();
    }

    #region Commands

    public ICommand DeleteSelectedTransactionsCommand { get; }

    #endregion

    public override void Dispose()
    {
        DisposeTransactionViewModels();
        _backupCommandStore.PropertyChanged -= OnBackupCommandStorePropertyChanged;
        base.Dispose();
    }

    #region Handle BackupCommandStore events

    private void OnBackupCommandStorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BackupCommandStore.BackupCommands))
        {
            LoadTransactionViewModels();
        }
    }

    #endregion

    #region Handle TransactionViewModel events

    private void OnTransactionViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TransactionViewModel.IsSelected))
        {
            OnPropertyChanged(nameof(SelectedTransactionIndices));
        }
    }

    #endregion

    #region TransactionViewModels

    public ObservableCollection<TransactionViewModel> Transactions { get; } = [];

    public List<int> SelectedTransactionIndices
    {
        get
        {
            List<int> indices = [];
            for (int i = 0; i < Transactions.Count; i++)
            {
                if (Transactions[i].IsSelected)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }
    }

    private void LoadTransactionViewModels()
    {
        DisposeTransactionViewModels();

        for (int i = 0; i < _backupCommandStore.BackupCommands.Count; i++)
        {
            BackupCommand backupCommand = _backupCommandStore.BackupCommands[i];

            int idx = i;

            TransactionViewModel transactionViewModel = new(backupCommand,
                () => _backupCommandStore.RunByIndex(idx),
                () => _backupCommandStore.PauseByIndex(idx),
                () => _backupCommandStore.StopByIndex(idx));

            transactionViewModel.PropertyChanged += OnTransactionViewModelPropertyChanged;
            Transactions.Add(transactionViewModel);
        }

        OnPropertyChanged(nameof(SelectedTransactionIndices));
    }

    /// <summary>
    ///     Get rid of all TransactionViewModels
    /// </summary>
    private void DisposeTransactionViewModels()
    {
        foreach (TransactionViewModel transactionViewModel in Transactions)
        {
            transactionViewModel.PropertyChanged -= OnTransactionViewModelPropertyChanged;
            transactionViewModel.Dispose();
        }

        Transactions.Clear();
    }

    #endregion
}
