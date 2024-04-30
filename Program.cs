using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using BandwidthBuddy.Scripts;

namespace BandwidthBuddy;

class Program
{
    private static readonly string[] serverAliases = { "--server", "-s" };

    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Bandwidth CLI - A CLI for basic bandwidth monitoring");

        Option<string> serverOption = new Option<string>(serverAliases, "Specify the server URL to test against (defaults to Google's homepage if not specified).");
        rootCommand.AddGlobalOption(serverOption);

        // Download speed test
        Command testDownloadSpeed = new Command("down", "Test Download Speed");
        testDownloadSpeed.SetHandler(async (string server) =>
        {
            Uri serverUri = ValidateAndConvertUri(server);
            double results = await SpeedTests.DownloadSpeedTest(serverUri);
            Console.WriteLine($"Download speed: {results:F2} Mbps");
        }, serverOption);
        rootCommand.AddCommand(testDownloadSpeed);

        // Upload speed test
        Command testUploadSpeed = new Command("up", "Test Upload Speed");
        testUploadSpeed.SetHandler(async (string server) =>
        {
            Uri serverUri = ValidateAndConvertUri(server);
            double results = await SpeedTests.UploadSpeedTest(serverUri);
            Console.WriteLine($"Upload speed: {results:F2} Mbps");
        }, serverOption);
        rootCommand.AddCommand(testUploadSpeed);

        // Estimate available bandwidth
        Command estimateBandwidth = new Command("est", "Estimate Available Bandwidth");
        estimateBandwidth.SetHandler(async (string server) =>
        {
            Uri serverUri = ValidateAndConvertUri(server);
            double results = await SpeedTests.EstimateAvailableBandwidth(serverUri);
            Console.WriteLine($"Estimated available bandwidth: {results:F2} Mbps");
        }, serverOption);
        rootCommand.AddCommand(estimateBandwidth);

        // Parse and invoke the command line arguments
        await rootCommand.InvokeAsync(args);
    }

    static Uri ValidateAndConvertUri(string server)
    {
        if (string.IsNullOrWhiteSpace(server))
        {
            server = "https://www.google.com";
        }
        else if (!server.StartsWith("http://") && !server.StartsWith("https://"))
        {
            server = "https://" + server;
        }
        return new Uri(server);
    }
}
