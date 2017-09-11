using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace SampleApp.Services
{
    public class WarmupService
    {
        public WarmupService(
            IApplicationLifetime appLifetime)
        {
            if (appLifetime == null) throw new ArgumentNullException(nameof(appLifetime));
            appLifetime.ApplicationStarted.Register(async () => await WarmupAsync().ConfigureAwait(false));
        }

        public async Task WarmupAsync()
        {
            const int cnt = 500;
            const int payloadLength = 1000;
            var payload = new string('B', payloadLength);

            Console.WriteLine("Warmup...");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
            if (string.IsNullOrEmpty(url) || url == "http://localhost:50614/")
                url = "http://localhost:50614";
            else
                url = "http://localhost:80";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;
                //client.DefaultRequestHeaders.Connection.Add("keep-alive");
                //client.DefaultRequestHeaders.ConnectionClose = false;
                //client.DefaultRequestHeaders.Connection.Add("keep-alive");

                // call endpoints
                for (int i = 0; i < cnt; i++)
                {
                    try
                    {
                        var fakeurl = $"{url}/api/values/fake/{payloadLength}";
                        using (var response = await client.PostAsync(
                            $"{url}/api/values/runpost",
                            new StringContent("\"" + fakeurl + "\"", Encoding.UTF8, "application/json")).ConfigureAwait(false))
                        {
                            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            if (result.StartsWith("Ex: "))
                            {
                                Console.WriteLine(result);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = $"Warming up error for #{i}: {ex.ToString()}";
                        Console.WriteLine(error);
                        break;
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"Warmup took: {sw.ElapsedMilliseconds} ms");
        }
    }
}
