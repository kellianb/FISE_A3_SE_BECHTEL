using System.Globalization;

namespace BackupUtil.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");
        return new BackupUtilCli().Invoke(args);
    }
}
