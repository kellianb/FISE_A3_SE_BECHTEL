using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.Store;

public class BackupCommandStore : INotifyPropertyChanged
{
    #region BackupCommands

    public ObservableCollection<BackupCommand> BackupCommands { get; } = [];

    #endregion

    #region PropertyChanged event handler

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
