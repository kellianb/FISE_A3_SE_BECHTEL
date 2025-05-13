using System.Text;
using BackupUtil.Core.Job;

namespace BackupUtil.Cli.Util;

internal static class DisplayJobs
{
    private static string DisplayOne(Job job, int index)
    {
        StringBuilder display = new();

        display.AppendLine(Formatting.BoldOn + "[" + index + "] " + job.Name + Formatting.Reset);
        display.AppendLine(Formatting.Indent + job.SourcePath);
        display.AppendLine(Formatting.Indent + job.TargetPath);
        display.AppendLine(Formatting.Indent + job.Recursive);
        display.AppendLine(Formatting.Indent + job.Differential);
        display.AppendLine();

        return display.ToString();
    }

    public static string Display(List<Job> jobs)
    {
        string display = "";

        foreach ((Job job, int idx) in jobs.Select((job, idx) => (job, idx)))
        {
            display += DisplayOne(job, idx);
        }

        return display;
    }
}
