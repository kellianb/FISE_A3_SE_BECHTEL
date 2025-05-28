using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.Store;

/// <summary>
/// Represents a store for managing backup commands. This class provides an observable collection
/// of <see cref="BackupCommand"/> objects and implements <see cref="INotifyPropertyChanged"/> to
/// notify clients of property changes.
/// </summary>
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
