using System.ComponentModel;
using BackupUtil.Core.Command;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.Transaction;

public class StopTransactionCommand : CommandBase
{
    private readonly Action _stop;
    private readonly TransactionViewModel _transactionViewModel;

    public StopTransactionCommand(TransactionViewModel transactionViewModel, Action stop)
    {
        _stop = stop;
        _transactionViewModel = transactionViewModel;
        _transactionViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_transactionViewModel.State))
        {
            OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return _transactionViewModel.State is BackupCommandState.Running
               && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _stop();
    }
}
