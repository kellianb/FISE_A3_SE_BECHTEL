using System.Text;
using BackupUtil.Core.Job;
using BackupUtil.I18n;

namespace BackupUtil.Cli.Util;

internal static class DisplayJobs
{
    private static string DisplayOne(Job job, int index)
    {
        StringBuilder display = new();

        const int indexWidth = -5; // Left-aligned, 5 chars wide
        const int labelWidth = -15; // Left-aligned, 15 chars wide
        const int valueWidth = -30; // Left-aligned, 30 chars wide

        display.AppendLine(Formatting.BoldOn
                           + $"{$"[{index}]",indexWidth}{I18N.GetLocalizedMessage("jobName"),labelWidth}{job.Name,valueWidth}"
                           + Formatting.Reset);

        void AppendDetailLine(string labelKey, object value)
        {
            display.AppendLine($"{"",-indexWidth}{I18N.GetLocalizedMessage(labelKey),labelWidth}{value,valueWidth}");
        }

        // Use the method for each detail line
        AppendDetailLine("jobSourcePath", job.SourcePath);
        AppendDetailLine("jobTargetPath", job.TargetPath);
        AppendDetailLine("jobRecursive", job.Recursive);
        AppendDetailLine("jobDifferential", job.Differential);
        AppendDetailLine("jobEncrypted", job.EncryptionKey != null);


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
