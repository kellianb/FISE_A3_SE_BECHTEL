using BackupUtil.Core.Command;

namespace BackupUtil.ViewModel.Store;

public class ProgramFilterStore(ProgramFilter programFilter)
{
    private ProgramFilter _programFilter = programFilter;

    public ProgramFilter ProgramFilter
    {
        get => _programFilter;
        set
        {
            _programFilter = value;
            OnProgramFilterChanged();
        }
    }

    public event Action? ProgramFilterChanged;

    private void OnProgramFilterChanged()
    {
        ProgramFilterChanged?.Invoke();
    }
}
