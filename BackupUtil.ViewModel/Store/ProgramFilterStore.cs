using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.Store;

public class ProgramFilterStore
{
    public ProgramFilterStore(ProgramFilter? programFilter = null)
    {
        ProgramFilter = programFilter ?? new ProgramFilter([]);
    }

    public ProgramFilter ProgramFilter { get; }

    public List<string> BannedPrograms
    {
        get => ProgramFilter.BannedPrograms;
        set
        {
            ProgramFilter.BannedPrograms = value;
            OnProgramFilterChanged();
        }
    }

    public event Action? ProgramFilterChanged;

    private void OnProgramFilterChanged()
    {
        ProgramFilterChanged?.Invoke();
    }
}
