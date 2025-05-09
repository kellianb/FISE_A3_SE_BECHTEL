namespace BackupUtil.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        return new BackupUtilCli().Invoke(args);
    }
}
