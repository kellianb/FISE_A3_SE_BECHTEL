using System.CommandLine;
using System.Text.Json;
using BackupUtil.Core.Executor;
using BackupUtil.Core.Job;
using BackupUtil.Core.Transaction;

namespace BackupUtil.Cli;

internal class BackupUtilCli
{
    private readonly RootCommand _command;

    public BackupUtilCli()
    {
        Argument<string> sourcePath = new("source-path", "Source path of the backup");
        Argument<string> targetPath = new("target-path", "Target path of the backup");
        Option<bool> recursive = new("--recursive", "Make the backup recursive");
        Option<bool> differential = new("--differential", "Make the backup differential");

        _command = [sourcePath, targetPath, recursive, differential];

        _command.SetHandler(CommandHandler,
            sourcePath,
            targetPath,
            recursive,
            differential
        );
    }

    public int Invoke(string[] args)
    {
        return _command.Invoke(args);
    }

    private static void CommandHandler(string sourcePath, string targetPath, bool recursive, bool differential)
    {
        Job job = new(sourcePath, targetPath, recursive, differential);

        try
        {
            BackupTransaction transaction = BackupTransactionBuilder.Build(job);

            Console.WriteLine("Making these changes");
            Console.WriteLine(JsonSerializer.Serialize(transaction, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("Is this ok [Y/n]");

            if (new List<string>(["Y", "y", ""]).Contains(Console.ReadLine() ?? "n"))
            {
                new BackupTransactionExecutor().Execute(transaction);
                Console.WriteLine("Done");
            }


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }

    }
}
