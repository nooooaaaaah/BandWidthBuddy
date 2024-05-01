using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace BandwidthBuddy.Scripts
{
    public static class SpeedTests
    {
        private static readonly HttpClient httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(60)  // Set a global 30-second timeout
        };

        public static async Task<double> DownloadSpeedTest(string serverUrl, int dataSizeInBytes)
        {
            string url = $"{ConvertToUri(serverUrl)}/bytes/{dataSizeInBytes * 100}";
            Console.WriteLine(url);
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                byte[] data = await response.Content.ReadAsByteArrayAsync();
                stopwatch.Stop();

                long bytesReceived = data.Length;
                double seconds = stopwatch.Elapsed.TotalSeconds;
                double downloadSpeedMbps = CalculateSpeed(bytesReceived, seconds);

                Console.WriteLine($"Download test elapsed time: {seconds} seconds.");
                Console.WriteLine($"Download Speed: {downloadSpeedMbps} MBps");
                return downloadSpeedMbps;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Network or HTTP error during download test: {e.Message}");
                return 0;
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timed out: {e.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during download test: {ex.Message}");
                return 0;
            }
        }

        public static async Task<double> UploadSpeedTest(string serverUrl, int dataSizeInBytes)
        {
            string url = $"{ConvertToUri(serverUrl)}/post";
            byte[] data = new byte[dataSizeInBytes];
            new Random().NextBytes(data);
            ByteArrayContent content = new ByteArrayContent(data);
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                stopwatch.Stop();

                long bytesSent = data.Length;
                double seconds = stopwatch.Elapsed.TotalSeconds;
                double uploadSpeedMbps = CalculateSpeed(bytesSent, seconds);
                Console.WriteLine($"Upload test elapsed time: {seconds} seconds.");
                Console.WriteLine($"Upload Speed: {uploadSpeedMbps} MBps");
                return uploadSpeedMbps;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Network or HTTP error during upload test: {e.Message}");
                return 0;
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timed out: {e.Message}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during upload test: {ex.Message}");
                return 0;
            }
        }

        public static async Task<double> EstimateAvailableBandwidth(string serverUrl, int dataSizeInBytes)
        {
            double downloadSpeedMbps = await DownloadSpeedTest(serverUrl, dataSizeInBytes);
            double uploadSpeedMbps = await UploadSpeedTest(serverUrl, dataSizeInBytes);

            double availableBandwidthMbps = Math.Min(downloadSpeedMbps, uploadSpeedMbps);
            Console.WriteLine($"Estimated Available Bandwidth: {availableBandwidthMbps:F5} MBps");
            return availableBandwidthMbps;
        }

        public static async Task<double> LatencyTest(string serverUrl)
        {
            string url = $"{ConvertToUri(serverUrl)}/ping";
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                stopwatch.Stop();

                double latencyInMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                Console.WriteLine($"Ping latency: {latencyInMilliseconds} ms.");
                return latencyInMilliseconds;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Network or HTTP error during latency test: {e.Message}");
                return double.MaxValue;  // Indicate an error with the maximum value
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"Request timed out: {e.Message}");
                return double.MaxValue;  // Indicate a timeout with the maximum value
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during latency test: {ex.Message}");
                return double.MaxValue;  // Indicate a general error with the maximum value
            }
        }

        private static double CalculateSpeed(long bytes, double seconds)
        {
            if (seconds <= 0)
            {
                Console.WriteLine("Elapsed time is zero or negative, cannot calculate speed.");
                return 0;
            }

            double bits = bytes * 8;
            double megabits = bits / 1_000_000.0;  // Convert bits to megabits using the standard 1 Megabit = 10^6 bits
            double speedMbps = megabits / seconds;
            double speedMBps = speedMbps / 8;  // Convert Mbps to MBps
            return speedMBps;
        }


        private static Uri ConvertToUri(string server)
        {
            if (!server.StartsWith("http://") && !server.StartsWith("https://"))
            {
                server = "http://" + server + ":8000";  // Defaulting to HTTPS for security
            }
            return new Uri(server);
        }
    }
}
