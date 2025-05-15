namespace BackupUtil.Core.Util;

public class UncPathConverter
{
    public static string GetUncPath(string p)
    {
    // Check if the path is already a UNC path
        if (p.StartsWith(@"\\"))
            return p;

        // Convert local path to UNC format
        try
        {
            // Get the full path to resolve any relative path components
            string fullPath = Path.GetFullPath(p);

            // Get the machine name
            string machineName = Environment.MachineName;

            // Convert to UNC format by prepending \\MachineName\
            return $@"\\{machineName}\{fullPath.Replace(':', '$')}";
        }
        catch (Exception)
        {
            // Return original path if conversion fails
            return p;
        }
    }
}
