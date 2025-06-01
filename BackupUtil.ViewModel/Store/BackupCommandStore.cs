using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.Store;

/// <summary>
///     Represents a store for managing backup commands. This class provides an observable collection
///     of <see cref="BackupCommand" /> objects and implements <see cref="INotifyPropertyChanged" /> to
///     notify clients of property changes.
/// </summary>
public class BackupCommandStore : INotifyPropertyChanged
{
    private readonly ProgramFilterStore _programFilterStore;

    public BackupCommandStore(ProgramFilterStore programFilterStore)
    {
        _programFilterStore = programFilterStore;
    }


    public List<BackupCommand> BackupCommands { get; } = [];

    #region Add BackupCommands

    public void AddBackupCommand(BackupCommand backupCommand)
    {
        BackupCommands.Add(backupCommand);
        OnPropertyChanged(nameof(BackupCommands));
    }

    #endregion

    #region Run BackupCommands

    public void RunByIndex(int index)
    {
        BackupCommand command = BackupCommands[index];
        command.ProgramFilter = _programFilterStore.ProgramFilter;
        command.Start();
    }

    #endregion

    #region Pause BackupCommands

    public void PauseByIndex(int index)
    {
        BackupCommands[index].Pause();
    }

    #endregion

    #region Stop BackupCommands

    public void StopByIndex(int index)
    {
        BackupCommands[index].Stop();
    }

    #endregion

    #region Remove BackupCommands

    public void RemoveByIndices(List<int> indices)
    {
        foreach (int index in indices.Distinct().OrderDescending())
        {
            BackupCommands.RemoveAt(index);
        }

        OnPropertyChanged(nameof(BackupCommands));
    }

    #endregion

    #region PropertyChanged event handler

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}
