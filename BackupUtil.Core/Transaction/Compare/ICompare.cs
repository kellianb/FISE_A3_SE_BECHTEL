using BackupUtil.Core.Transaction.Editor;

namespace BackupUtil.Core.Transaction.Compare;

internal interface ICompare
{
    public BackupTransactionEditor Compare(BackupTransactionEditor transaction);
}
