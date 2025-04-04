namespace ScreenMonitor;

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            Console.WriteLine("No arguments provided. Use 'capture', 'compile' or 'clean'.");
            return;
        }

        var argument = args[0];
        Console.WriteLine($"Started at {DateTime.Now} with argument {argument}");

        switch (argument.ToLowerInvariant())
        {
            case "capture":
                Capture.Run();
                break;

            case "compile":
                var includeNow = args.Length >= 2
                    && args[1].Equals("now", StringComparison.InvariantCultureIgnoreCase);
                
                Compile.Run(includeNow);
                break;

            case "clean":
                Clean.Run();
                break;
            
            default:
                Console.WriteLine("Invalid argument. Use 'capture', 'compile' or 'clean'.");
                break;
        }

        Console.WriteLine($"Finished at {DateTime.Now}");
    }
}