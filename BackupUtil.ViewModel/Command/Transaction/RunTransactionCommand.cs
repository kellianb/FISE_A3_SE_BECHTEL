using System.ComponentModel;
using BackupUtil.Core.Command;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.Transaction;

public class RunTransactionCommand : CommandBase
{
    private readonly Action _run;
    private readonly TransactionViewModel _transactionViewModel;

    public RunTransactionCommand(TransactionViewModel transactionViewModel, Action run)
    {
        _run = run;
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
        return _transactionViewModel.State
                   is BackupCommandState.NotStarted
                   or BackupCommandState.Paused
               && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        _run();
    }
}
