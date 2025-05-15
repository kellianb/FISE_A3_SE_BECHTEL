using System.CommandLine;
using BackupUtil.Cli.Util;
using BackupUtil.Core.Job;
using BackupUtil.Core.Util;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Command;

public class RemoveJobCommand
{
    public static System.CommandLine.Command Build()
    {
        // Path of the job file
        Option<FileSystemInfo> jobFilePath = new(["--job-file-path", "-o"], "Job file path");

        System.CommandLine.Command command = new("remove", "Remove a backup job from a file");

        command.AddOption(jobFilePath);

        command.SetHandler(CommandHandler,
            jobFilePath
        );

        return command;
    }

    private static void CommandHandler(FileSystemInfo jobFilePath)
    {
        try
        {
            JobManager jobManager = new JobManager().AddJobsFromFile(jobFilePath?.FullName);

            // Show loaded jobs
            Console.Write(DisplayJobs.Display(jobManager.Jobs));

            // Ask the user to select which ones to remove
            Console.WriteLine(I18N.GetLocalizedMessage("selectJobsToRemove"));

            string selection = Console.ReadLine() ?? "";

            Console.WriteLine();

            HashSet<int> selectedIndices = SelectionStringParser.Parse(selection);

            jobManager.RemoveByIndices(selectedIndices);

            Console.Write(DisplayJobs.Display(jobManager.Jobs));

            // Ask the user if this is ok
            Console.WriteLine(I18N.GetLocalizedMessage("IsOk"));

            if (Console.ReadLine() is "Y" or "y" or "")
            {
                jobManager.ExportAll(jobFilePath?.FullName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(I18N.GetLocalizedMessage(e.Message));
        }
    }
}
