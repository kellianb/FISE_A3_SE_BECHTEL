using System.CommandLine;
using BackupUtil.Cli.Command;

namespace BackupUtil.Cli;

internal class Program
{
    private static int Main(string[] args)
    {
        RootCommand rootCommand =
        [
            SingleJobCommand.Build(),
            LoadJobsCommand.Build()
        ];

        return rootCommand.Invoke(args);
    }
}
