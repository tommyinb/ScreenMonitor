using System.Text.RegularExpressions;

namespace ScreenMonitor;

static partial class Clean
{
    public static void Run()
    {
        var folders = GetFolders();

        Console.WriteLine($"Delete {folders.Length} folders.");

        foreach (var folder in folders)
        {
            Console.WriteLine($"Delete {folder} folder...");
        
            try
            {
                Directory.Delete(folder, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting folder {folder}: {ex.Message}");
            }
        }
    }

    private static string[] GetFolders()
    {
        var folders = Directory
            .GetDirectories(".", "Captured_*")
            .Select(path => Path.GetFileName(path));

        var regex = FolderRegex();
        var hourTexts = folders
            .Select(folder => regex.Match(folder))
            .Where(match => match.Success)
            .Select(match => match.Groups[1].Value);

        var fromText = DateTime.Now.AddDays(-1).ToString("yyMMddHH");

        return [.. hourTexts
            .Where(hourText => string.Compare(hourText, fromText) < 0)
            .Select(hourText => $"Captured_{hourText}")];
    }
    [GeneratedRegex(@"^Captured_(\d{8})$")]
    private static partial Regex FolderRegex();
}