// Purpose: Contains the SpeedTests class, which contains methods for testing download and upload speeds.
using System.Diagnostics;
namespace BandwidthBuddy.Scripts;

class SpeedTests
{
    public static async Task<double> DownloadSpeedTest(Uri serverUrl)
    {
        using (HttpClient httpClient = new())
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpResponseMessage response = await httpClient.GetAsync(serverUrl);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStreamAsync();
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            return response.Content.Headers.ContentLength / elapsed.TotalSeconds / 1024 / 1024;
            // Console.WriteLine($"Download speed: {response.Content.Headers.ContentLength / elapsed.TotalSeconds / 1024 / 1024} MB/s");
        }
    }

    public static async Task<double> UploadSpeedTest(Uri serverUrl)
    {
        using (HttpClient httpClient = new())
        {
            byte[] content = new byte[10 * 1024 * 1024]; // 1 MB of data for upload test
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await httpClient.PostAsync(serverUrl, new ByteArrayContent(content));
            stopwatch.Stop();
            TimeSpan elapsed = stopwatch.Elapsed;
            return content.Length / elapsed.TotalSeconds / 1024 / 1024;
            // Console.WriteLine($"Upload speed: {content.Length / elapsed.TotalSeconds / 1024 / 1024} MB/s");
        }
    }

    public static async Task<double> EstimateAvailableBandwidth(Uri serverUrl)
    {
        double downloadSpeedMbps = await DownloadSpeedTest(serverUrl);
        double uploadSpeedMbps = await UploadSpeedTest(serverUrl);

        // Calculate available bandwidth as the minimum of download and upload speeds
        double availableBandwidthMbps = Math.Min(downloadSpeedMbps, uploadSpeedMbps);

        return availableBandwidthMbps;
    }
}