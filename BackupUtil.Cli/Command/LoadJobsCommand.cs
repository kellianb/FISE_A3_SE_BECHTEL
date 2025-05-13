using System.CommandLine;
using BackupUtil.Cli.Util;
using BackupUtil.Core.Command;
using BackupUtil.Core.Job;
using BackupUtil.Core.Util;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Command;

internal static class LoadJobsCommand
{
    public static System.CommandLine.Command Build()
    {
        Argument<FileSystemInfo?> jobFilePath = new("job-file-path", getDefaultValue: () => null ,"File path to load the jobs from");

        System.CommandLine.Command command = new("load", "Load backup jobs from a file and execute them");

        command.AddArgument(jobFilePath);

        command.SetHandler(CommandHandler,
            jobFilePath
        );

        return command;
    }

    private static void CommandHandler(FileSystemInfo? jobFilePath)
    {
        try
        {
            JobManager jobManager = new JobManager().AddJobsFromFile(jobFilePath?.FullName);

            // Show loaded jobs
            Console.Write(DisplayJobs.Display(jobManager.Jobs));

            // Ask the user to select which ones to run
            Console.WriteLine(I18N.GetLocalizedMessage("selectJobs"));

            string selection = Console.ReadLine() ?? "";

            Console.WriteLine();

            HashSet<int> selectedIndices = SelectionStringParser.Parse(selection);

            BackupCommand command = jobManager.RunByIndices(selectedIndices);

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
