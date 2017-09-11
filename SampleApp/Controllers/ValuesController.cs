using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System;
using Microsoft.Extensions.Logging;

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
        [Route("fake/{length:int}")]
        public async Task<string> GetFake(int length)
        {
            await Task.Delay(100);
            return new string('A', length);
        }

        // POST api/values
        [HttpPost]
        [Route("post")]
        public async Task<string> PostToPostAsync([FromBody]string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;
                using (var response = await client.PostAsync(
                    url, 
                    new StringContent(url, Encoding.UTF8, "application/json")).ConfigureAwait(false))
                {
                    try
                    {
                        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                        return "Ex: " + ex.ToString();
                    }
                }
            }
        }

        // POST api/values
        [HttpPost]
        [Route("runpost")]
        public string PostToRunPostAsync([FromBody]string url)
        {
            var t = Task.Run(async() =>
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Connection.Clear();
                    client.DefaultRequestHeaders.ConnectionClose = true;
                    using (var response = await client.PostAsync(
                        url,
                        new StringContent(url, Encoding.UTF8, "application/json")).ConfigureAwait(false))
                    {
                        try
                        {
                            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                            return result;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                            return "Ex: " + ex.ToString();
                        }
                    }
                }
            });
            return "accepted";
        }

        // POST api/values/get
        [HttpPost]
        [Route("get")]
        public async Task<string> PostToGetAsync([FromBody]string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Connection.Clear();
                client.DefaultRequestHeaders.ConnectionClose = true;
                using (var response = await client.GetAsync(url).ConfigureAwait(false))
                {
                    try
                    {
                        var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(new EventId(ex.HResult), ex, ex.Message);
                        return "Ex: " + ex.ToString();
                    }
                }
            }
        }
    }
}
