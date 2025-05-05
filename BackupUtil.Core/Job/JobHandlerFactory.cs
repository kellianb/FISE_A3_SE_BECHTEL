using BackupUtil.Core.Job.CopyHandler;

namespace BackupUtil.Core.Job;

public static class JobHandlerFactory
{
    public static ICopyHandler GetHandler(Job job)
    {
        if (File.Exists(job.SourcePath))
        {
            if (job.TargetPath.EndsWith('\\'))
            {
                throw new ArgumentException("Target file path must not end with a '\\'");
            }

            return new SingleFileHandler(new FileInfo(job.SourcePath), job.TargetPath);
        }

        if (Directory.Exists(job.SourcePath))
        {
            return new DirectoryHandler(new DirectoryInfo(job.SourcePath), job.TargetPath, job.Recursive);
        }

        throw new FileNotFoundException("Source file or directory not found");
    }
}
