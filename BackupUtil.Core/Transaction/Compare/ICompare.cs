namespace BackupUtil.Core.Transaction.Compare;

internal interface ICompare
{
    public BackupTransaction Compare(BackupTransaction transaction);
}
