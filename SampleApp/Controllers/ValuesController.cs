using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace SampleApp.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "healthy";
        }

        // GET api/values/fake/0
        [HttpGet]
        [HttpPost]
        [Route("fake/{n:int}")]
        public string GetFake(int n)
        {
            return $"fake-{n}";
        }

        // POST api/values
        [HttpPost]
        [Route("post")]
        public async Task<string> PostToPostAsync([FromBody]string url)
        {
            try
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
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ConfigureAwait(false))
                        {
                            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _logger.LogTrace($"OK: {url}");
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, $"FAIL: {url}, {ex.Message}");
                return "Ex: " + ex.ToString();
            }
        }

        // POST api/values
        [HttpPost]
        [Route("runpost")]
        public string PostToRunPostAsync([FromBody]string url)
        {
            var t = Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Connection.Clear();
                        client.DefaultRequestHeaders.ConnectionClose = true;

                        var request = new HttpRequestMessage(HttpMethod.Post, url)
                        {
                            Content = new StringContent(url, Encoding.UTF8, "application/json")
                        };

                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).ConfigureAwait(false))
                        {
                            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _logger.LogTrace($"OK: {url}");
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex, $"FAIL: {url}, {ex.Message}");
                    return "Ex: " + ex.ToString();
                }
            });
            return "accepted";
        }

        // POST api/values/get
        [HttpPost]
        [Route("get")]
        public async Task<string> PostToGetAsync([FromBody]string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Connection.Clear();
                    client.DefaultRequestHeaders.ConnectionClose = true;
                    using (var response = await client.GetAsync(url).ConfigureAwait(false))
                    {
                        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        _logger.LogTrace($"OK: {url}");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(ex.HResult), ex, $"FAIL: {url}, {ex.Message}");
                return "Ex: " + ex.ToString();
            }
        }

        // POST api/values/get
        [HttpPost]
        [Route("runget")]
        public string PostToRunGetAsync([FromBody]string url)
        {
            var t = Task.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Connection.Clear();
                        client.DefaultRequestHeaders.ConnectionClose = true;
                        using (var response = await client.GetAsync(url).ConfigureAwait(false))
                        {
                            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _logger.LogTrace($"OK: {url}");
                            return result;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(new EventId(ex.HResult), ex, $"FAIL: {url}, {ex.Message}");
                    return "Ex: " + ex.ToString();
                }
            });
            return "accepted";
        }

    }
}
