using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int cnt = 1_000;
            for(int i = 0; i < cnt; i++)
            {
                // "System.Net.Http.HttpRequestException: Error while copying content to a stream." is reproduced, only in case server is running as self-host.
                // If server is running under IISExpress - there no such error, but only: 
                // "System.Net.Http.WinHttpException: Not enough storage is available to process this command"
                // which reproduced in both cases, when there about 200 outgoing requests or more.
                PostOne($"http://localhost:50614/api/values/fake/{i}", i, Console.Out);
                
                // Post to external site for comparison:
                //PostOne($"http://httpbin.org/post", i, Console.Out);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void PostOne(string url, int i, TextWriter writer)
        {
            var t = Task.Run(async () =>
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Connection.Clear();
                    client.DefaultRequestHeaders.ConnectionClose = true;

                    using (var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(url, Encoding.UTF8, "application/json")
                    })
                    {
                        try
                        {
                            using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ConfigureAwait(false))
                            {
                                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                                await writer.WriteLineAsync($"OK: {i}").ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            await writer.WriteLineAsync($"FAIL: {i}. EX: {ex}").ConfigureAwait(false);
                        }
                    }
                }
            });
        }
    }
}
