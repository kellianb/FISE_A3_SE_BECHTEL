using System.Text.Json;

namespace BackupUtil.Core.Job.FileLoader;

public class JsonLoader(string path) : IJobLoader
{
    public List<Job> LoadJobs()
    {
        Console.WriteLine($"Fetching JSON from {Path.GetFullPath(path)}");

        using StreamReader reader = new(path);

        return JsonSerializer.Deserialize<List<Job>>(reader.ReadToEnd()) ?? [];
    }
}
