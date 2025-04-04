using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace ScreenMonitor;

static class Capture
{
    public static void Run()
    {
        var timeValue = DateTime.Now;
        var timeText = timeValue.ToString("yyyy/MM/dd HH:mm:ss");

        var folderName = $"Captured_{timeValue:yyMMddHH}";
        Directory.CreateDirectory(folderName);

        foreach (Screen screen in Screen.AllScreens)
        {
            using var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, bitmap.Size);

                graphics.DrawString(timeText, new Font("Arial", 16), Brushes.Red, 10, screen.Bounds.Height - 50);
            }

            var displayName = screen.DeviceName.Replace("\\", "").Replace(".", "");
            var fileName = GetNextName(folderName, displayName);

            var filePath = Path.Combine(folderName, fileName);
            bitmap.Save(filePath, ImageFormat.Png);
            
            Console.WriteLine("Captured " + fileName);
        }
    }

    private static string GetNextName(string folderName, string displayName)
    {
        var fileNames = Directory
            .GetFiles(folderName, $"{displayName}*.png")
            .Select(filePath => Path.GetFileName(filePath))
            .OrderDescending();

        var regex = new Regex($@"{displayName}_(\d+)\.png$");
        var indexes = fileNames
            .Select(fileName => regex.Match(fileName))
            .Where(match => match.Success)
            .Select(match => int.Parse(match.Groups[1].Value));

        var index = indexes.Any() ? indexes.Max() : 0;
        
        return $"{displayName}_{index + 1:000}.png";
    }
}