// Purpose: Provides methods for testing download and upload speeds.
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandwidthBuddy.Scripts;

public static class SpeedTests
{
    private static readonly HttpClient httpClient = new HttpClient(); // Reuse HttpClient across requests

    public static async Task<double> DownloadSpeedTest(Uri serverUrl)
    {
        Stopwatch stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            using (HttpResponseMessage response = await httpClient.GetAsync(serverUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                long? contentLength = response.Content.Headers.ContentLength;
                await response.Content.ReadAsStreamAsync(); // Ensure full download for accurate timing
                stopwatch.Stop();
                return CalculateSpeed(contentLength, stopwatch.Elapsed);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during download test: {ex.Message}");
            return 0;
        }
    }

    public static async Task<double> UploadSpeedTest(Uri serverUrl)
    {
        byte[] content = new byte[10 * 1024 * 1024]; // 10 MB of data for upload test
        Stopwatch stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            using (ByteArrayContent byteContent = new ByteArrayContent(content))
            {
                await httpClient.PostAsync(serverUrl, byteContent);
                stopwatch.Stop();
                return CalculateSpeed(content.Length, stopwatch.Elapsed);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during upload test: {ex.Message}");
            return 0;
        }
    }

    public static async Task<double> EstimateAvailableBandwidth(Uri serverUrl)
    {
        double downloadSpeedMbps = await DownloadSpeedTest(serverUrl);
        double uploadSpeedMbps = await UploadSpeedTest(serverUrl);
        return Math.Min(downloadSpeedMbps, uploadSpeedMbps); // Return the minimum of download and upload speeds
    }

    private static double CalculateSpeed(long? bytes, TimeSpan elapsed)
    {
        if (!bytes.HasValue)
        {
            return 0;
        }

        // Convert bytes per second to megabits per second
        return (bytes.Value * 8) / (1024.0 * 1024.0 * elapsed.TotalSeconds);
    }
}
