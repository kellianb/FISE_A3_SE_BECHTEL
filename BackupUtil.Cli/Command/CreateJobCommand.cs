using System.CommandLine;
using BackupUtil.Core.Job;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Command;

internal static class CreateJobCommand
{
    public static System.CommandLine.Command Build()
    {
        Argument<FileSystemInfo> sourcePath = new("source-path", "Source path of the backup");
        Argument<FileSystemInfo> targetPath = new("target-path", "Target path of the backup");
        Option<string> name = new(["--name", "-n"], "Name of the job");
        Option<bool> recursive = new(["--recursive", "-r"], "Make the backup recursive");
        Option<bool> differential = new(["--differential", "-d"], "Make the backup differential.");

        // Path of the job file
        Option<FileSystemInfo> jobFilePath = new(["--job-file-path", "-o"], "Job file path");

        System.CommandLine.Command command = new("create", "Create a backup job and write it to a file");

        command.AddArgument(sourcePath);
        command.AddArgument(targetPath);
        command.AddOption(name);
        command.AddOption(recursive);
        command.AddOption(differential);
        command.AddOption(jobFilePath);

        command.SetHandler(CommandHandler,
            sourcePath,
            targetPath,
            recursive,
            differential,
            name,
            jobFilePath
        );

        return command;
    }

    private static void CommandHandler(FileSystemInfo sourcePath, FileSystemInfo targetPath, bool recursive,
        bool differential, string? name = null, FileSystemInfo? jobFilePath = null)
    {
        Job job = new(sourcePath.FullName, targetPath.FullName, recursive, differential, name);

        try
        {
            JobManager manager = new();

            try
            {
                manager.AddJobsFromFile(jobFilePath?.FullName);
            }
            catch
            {
                // Only catch the exception if the default job file path was used
                if (jobFilePath is not null)
                {
                    throw;
                }
            }

            manager.AddJob(job)
                .ExportAll(jobFilePath?.FullName);
        }
        catch (Exception e)
        {
            Console.WriteLine(I18N.GetLocalizedMessage(e.Message));
        }
    }
}
