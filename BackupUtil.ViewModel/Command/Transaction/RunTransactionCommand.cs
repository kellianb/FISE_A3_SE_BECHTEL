using System.ComponentModel;
using BackupUtil.Core.Command;
using BackupUtil.Core.Util;
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
                   or BackupCommandState.PausedError
                   or BackupCommandState.PausedBannedProgram
               && base.CanExecute(parameter);
    }

    public override void Execute(object? parameter)
    {
        try
        {
            _run();
        }
        catch (Exception e)
        {
            Logging.StatusLog.Value.Error("Encountered exception in {@string}: {@Exception}", GetType().Name, e);
        }
    }
}
