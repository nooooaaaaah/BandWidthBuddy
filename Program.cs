using System.CommandLine;
using BandwidthBuddy.Scripts;
namespace BandwidthBuddy;
class Program
{
    internal static readonly string[] serverAliases = { "--server", "-s" };
    internal static readonly int BYTES = 100 * 1024 * 1024;
    internal static readonly string defaultServer = "https://httpbin.org/";

    static async Task Main(string[] args)
    {
        RootCommand rootCommand = new("Bandwidth CLI");
        rootCommand.Description = "A CLI tool for monitoring and testing network bandwidth, including download and upload speeds, latency, and bandwidth estimation.";

        Option<string> serverOption = new(serverAliases, description: "The server to test against", getDefaultValue: () => defaultServer);
        rootCommand.AddGlobalOption(serverOption);

        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("Welcome to BandwidthBuddy! Use this CLI to monitor and test your network's performance.");
            Console.WriteLine("\nAvailable Commands:");
            foreach (var command in rootCommand.Children)
            {
                Console.WriteLine($"- {command.Name}: {command.Description}");
            }
        });

        // Download speed test
        Command testDownloadSpeed = new("down", "Test Download Speeds");
        testDownloadSpeed.Description = "Performs a download speed test against a specified server.";
        rootCommand.Add(testDownloadSpeed);
        testDownloadSpeed.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? defaultServer;
            await SpeedTests.DownloadSpeedTest(server, BYTES);
            Console.WriteLine("Download speed test completed.");
        });

        // Upload speed test
        Command testUploadSpeed = new("up", "Test Upload Speeds");
        testUploadSpeed.Description = "Performs an upload speed test against a specified server.";
        rootCommand.Add(testUploadSpeed);
        testUploadSpeed.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? defaultServer;
            await SpeedTests.UploadSpeedTest(server, BYTES);
            Console.WriteLine("Upload speed test completed.");
        });

        // Estimate available bandwidth
        Command estimateBandwidth = new("est", "Estimate Available Bandwidth");
        estimateBandwidth.Description = "Estimates the available bandwidth to the specified server.";
        rootCommand.Add(estimateBandwidth);
        estimateBandwidth.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? defaultServer;
            await SpeedTests.EstimateAvailableBandwidth(server, BYTES);
            Console.WriteLine("Bandwidth estimation completed.");
        });

        // Add a new command for latency test
        Command testLatency = new("ping", "Test Network Latency");
        testLatency.Description = "Tests the network latency to the specified server.";
        rootCommand.Add(testLatency);
        testLatency.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? defaultServer;
            await SpeedTests.LatencyTest(server);
            Console.WriteLine("Latency test completed.");
        });

        if (args.Length == 0)
        {
            rootCommand.Invoke("-h");
            return;
        }

        await rootCommand.InvokeAsync(args);
    }
}
