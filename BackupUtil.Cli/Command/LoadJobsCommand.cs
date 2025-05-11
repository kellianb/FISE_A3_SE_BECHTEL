using System.CommandLine;
using BackupUtil.Cli.Util;
using BackupUtil.Core.Command;
using BackupUtil.Core.Job;
using BackupUtil.Core.Util;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Command;

public class LoadJobsCommand
{
    public static System.CommandLine.Command Build()
    {
        Argument<string> jobFilePath = new("job-file-path", "File path to load the jobs from");

        System.CommandLine.Command command = new("load", "Load jobs from a file and execute them");

        command.AddArgument(jobFilePath);

        command.SetHandler(CommandHandler,
            jobFilePath
        );

        return command;
    }

    private static void CommandHandler(string jobFilePath)
    {
        try
        {
            JobManager jobManager = new JobManager().LoadJobsFromFile(jobFilePath);

            // Show loaded jobs
            Console.Write(DisplayJobs.Display(jobManager.Jobs));

            // Ask the user to select which ones to run
            Console.WriteLine(I18N.GetLocalizedMessage("selectJobs"));

            string selection = Console.ReadLine() ?? "";

            Console.WriteLine();

            HashSet<int> selectedIndexes = SelectionStringParser.Parse(selection);

            BackupCommand command = jobManager.GetBackupCommandForIndexes(selectedIndexes);

            // Display planned changes to the user
            Console.Write(DisplayChanges.DisplayDirectoryChanges(command.GetConcernedDirectories()));
            Console.Write(DisplayChanges.DisplayFileChanges(command.GetConcernedFiles()));

            // Ask the user if this is ok
            Console.WriteLine(I18N.GetLocalizedMessage("IsOk"));

            if (new List<string>(["Y", "y", ""]).Contains(Console.ReadLine() ?? "n"))
            {
                command.Execute();
                Console.WriteLine("Done");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(I18N.GetLocalizedMessage(e.Message));
        }
    }
}
