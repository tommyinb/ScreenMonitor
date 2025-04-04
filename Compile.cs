using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ScreenMonitor;

static partial class Compile
{
    public static void Run(bool includeNow)
    {
        Directory.CreateDirectory("Video");

        var folders = GetFolders(includeNow);
        foreach (var folder in folders)
        {
            Console.WriteLine($"Compile {folder} folder...");
            RunFolder(folder);
        }
    }
    public static IEnumerable<string> GetFolders(bool includeNow)
    {
        var folders = Directory
            .GetDirectories(".", "Captured_*")
            .Select(path => Path.GetFileName(path))
            .OrderDescending();

        Console.WriteLine($"Found {folders.Count()} folders.");
        
        if (includeNow)
        {
            return folders;
        }
        else
        {
            var nowName = $"Captured_{DateTime.Now:yyMMddHH}";
            return folders.Where(folder => !folder.Contains(nowName));
        }
    }

    private static void RunFolder(string folder)
    {
        var files = Directory.GetFiles(folder, "*.png");

        var regex = FileRegex();
        var displays = files
            .Select(file => Path.GetFileName(file))
            .Select(file => regex.Match(file))
            .Where(match => match.Success)
            .Select(match => match.Groups[1].Value)
            .Distinct()
            .ToArray();

        Console.WriteLine($"Found {displays.Length} displays in {folder} folder.");

        foreach (var display in displays)
        {
            Console.WriteLine($"Compile {display} display...");
            
            try
            {
                RunDisplay(folder, display);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing display {display}: {ex.Message}");
            }
        }
    }
    [GeneratedRegex(@"^([^_\\]+)_\d{3}\.png$")]
    private static partial Regex FileRegex();

    private static void RunDisplay(string folder, string display)
    {
        var outputPath = Path.Combine("Video", $"{folder}_{display}.mp4");
        if (File.Exists(outputPath))
        {
            Console.WriteLine($"Skip video {outputPath} which already exists.");
            return;
        }

        var inputPattern = Path.Combine(folder, $"{display}_%03d.png");
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-framerate 10 -i \"{inputPattern}\" -c:v libx264 -pix_fmt yuv420p \"{outputPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetFullPath(".")
            };

            using var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit(60 * 1000);

            if (!process.HasExited)
            {
                process.Kill();
                throw new TimeoutException("FFmpeg process timed out.");
            }

            Console.WriteLine("Video successfully created");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while creating video: " + ex.Message);
        }
    }
}