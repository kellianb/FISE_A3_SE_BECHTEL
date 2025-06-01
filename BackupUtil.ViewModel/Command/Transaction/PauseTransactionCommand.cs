using System.ComponentModel;
using BackupUtil.Core.Command;
using BackupUtil.Core.Util;
using BackupUtil.ViewModel.ViewModel;

namespace BackupUtil.ViewModel.Command.Transaction;

public class PauseTransactionCommand : CommandBase
{
    private readonly Action _pause;
    private readonly TransactionViewModel _transactionViewModel;

    public PauseTransactionCommand(TransactionViewModel transactionViewModel, Action pause)
    {
        _pause = pause;
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
        try
        {
            _pause();
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }
    }
}
