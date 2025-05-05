using BackupUtil.Core.Transaction.ChangeType;

namespace BackupUtil.Core.Transaction.Compare;

public interface ICompare
{
    public BackupTransaction Compare();
}
