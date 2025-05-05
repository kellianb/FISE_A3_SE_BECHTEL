namespace BackupUtil.Core.Job;

public class Job
{
    public Job(string sourcePath,
        string targetPath,
        bool recursive = false,
        bool differential = false,
        string? name = null,
        List<string>? ignorePatterns = null)
    {
        Name = name ?? "New backup job";
        SourcePath = sourcePath;
        TargetPath = targetPath;
        Recursive = recursive;
        Differential = differential;
        IgnorePatterns = ignorePatterns ?? [];
    }

    public Job()
    {
        Name = "New backup job";
        SourcePath = "";
        TargetPath = "";
        Recursive = false;
        Differential = false;
        IgnorePatterns = [];
    }

    public string Name { get; set; }
    public string SourcePath { get; set; }
    public string TargetPath { get; set; }

    public bool Recursive { get; set; }

    public bool Differential { get; set; }
    public List<string> IgnorePatterns { get; set; }
}
