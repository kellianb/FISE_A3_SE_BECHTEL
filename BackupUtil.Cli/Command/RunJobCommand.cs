using System.CommandLine;
using BackupUtil.Cli.Util;
using BackupUtil.Core.Command;
using BackupUtil.Core.Job;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Command;

internal static class RunJobCommand
{
    public static System.CommandLine.Command Build()
    {
        Argument<FileSystemInfo> sourcePath = new("source-path", "Source path of the backup");
        Argument<FileSystemInfo> targetPath = new("target-path", "Target path of the backup");
        Option<bool> recursive = new(["--recursive", "-r"], "Make the backup recursive");
        Option<bool> differential = new(["--differential", "-d"], "Make the backup differential.");

        System.CommandLine.Command command = new("run", "Run a backup job");

        command.AddArgument(sourcePath);
        command.AddArgument(targetPath);
        command.AddOption(recursive);
        command.AddOption(differential);

        command.SetHandler(CommandHandler,
            sourcePath,
            targetPath,
            recursive,
            differential
        );

        return command;
    }

    private static void CommandHandler(FileSystemInfo sourcePath, FileSystemInfo targetPath, bool recursive,
        bool differential)
    {
        Job job = new(sourcePath.FullName, targetPath.FullName, recursive, differential);

        try
        {
            BackupCommand command = new JobManager().AddJob(job).RunAll();

            // Display planned changes to the user
            Console.Write(DisplayChanges.DisplayDirectoryChanges(command.GetConcernedDirectories()));
            Console.Write(DisplayChanges.DisplayFileChanges(command.GetConcernedFiles()));

            // Ask the user if this is ok
            Console.WriteLine(I18N.GetLocalizedMessage("IsOk"));

            if (Console.ReadLine() is "Y" or "y" or "")
            {
                command.Execute();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(I18N.GetLocalizedMessage(e.Message));
        }
    }
}
