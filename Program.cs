using System;
using System.Diagnostics;
using System.Net.Http;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using BandwidthBuddy.Scripts;
namespace BandwidthBuddy;
class Program
{
    internal static readonly string[] serverAliases = ["--server", "-s"];
    

    static async Task Main(string[] args)
    {
        RootCommand rootCommand = new("Bandwidth CLI");

        Option<string> serverOption = new(serverAliases, "The server to test against");
        rootCommand.AddGlobalOption(serverOption);

        rootCommand.SetHandler(() =>
        {
            Console.WriteLine("A CLI for basic bandwidth monitoring");
        });

        // Download speed test
        Command testDownloadSpeed = new("ds", "Test Download Speeds");
        rootCommand.Add(testDownloadSpeed);
        testDownloadSpeed.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? "https://www.google.com";
            if (!server.StartsWith("http://") && !server.StartsWith("https://"))
            {
                server = "https://" + server;
            }
            await SpeedTests.DownloadSpeedTest(ConvertToUri(server));
        });
        
        // Upload speed test
        Command testUploadSpeed = new("us", "Test Upload Speeds");
        rootCommand.Add(testUploadSpeed);
        testUploadSpeed.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? "https://www.google.com";
            await SpeedTests.UploadSpeedTest(ConvertToUri(server));
        });

        // Estimate available bandwidth
        Command estimateBandwidth = new("eb", "Estimate Available Bandwidth");
        rootCommand.Add(estimateBandwidth);
        estimateBandwidth.SetHandler(async (context) =>
        {
            string server = context.ParseResult.GetValueForOption(serverOption) ?? "https://www.google.com";
            await SpeedTests.EstimateAvailableBandwidth(ConvertToUri(server));
        });
        await rootCommand.InvokeAsync(args);

    }

    static Uri ConvertToUri(string server)
    {
        if (!server.StartsWith("http://") && !server.StartsWith("https://"))
        {
            server = "https://" + server;
        }
        return new Uri(server);
    }
}
