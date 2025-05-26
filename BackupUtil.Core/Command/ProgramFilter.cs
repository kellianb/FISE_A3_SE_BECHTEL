using System.Diagnostics;

namespace BackupUtil.Core.Command;

public class ProgramFilter(List<string> bannedPrograms)
{
    public List<string> BannedPrograms { get; set; } = bannedPrograms;

    /// <summary>
    ///     Check if a banned program is running
    /// </summary>
    /// <exception cref="BannedProgramRunningException">a banned program is running</exception>
    public void CheckForBannedPrograms()
    {
        string? bannedProgramName = BannedPrograms.Find((x) => RunningProcesses.Contains(x));

        if (bannedProgramName is null)
        {
            return;
        }

        throw new BannedProgramRunningException("errorBannedProgramRunning", bannedProgramName);
    }

    public static HashSet<string> RunningProcesses => Process.GetProcesses()
        .Select(p => p.ProcessName.ToLower())
        .ToHashSet();
}

public class BannedProgramRunningException : Exception
{
    internal BannedProgramRunningException(string message, string bannedProgramName) : base(message)
    {
        BannedProgramName = bannedProgramName;
    }

    public string BannedProgramName { get; }
}
