using BackupUtil.Core.Operation.ChangeType;

namespace BackupUtil.Core.Operation.Compare;

public interface ICompare
{
    public Transaction Compare();
}
